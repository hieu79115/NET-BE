﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NET_BE.Data;
using NET_BE.DTOs;
using NET_BE.Model;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using NET_BE.Helpers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AttendanceDbContext _context;
    private readonly IConfiguration _configuration;

    [Authorize(Roles = "Student")]
    [HttpGet("secure-student-endpoint")]
    public IActionResult TestStudent() => Ok("Student authenticated!");

    [Authorize(Roles = "Lecturer")]
    [HttpGet("secure-lecturer-endpoint")]
    public IActionResult TestLecturer() => Ok("Lecturer authenticated!");

    public AuthController(AttendanceDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto login)
    {
        if (string.IsNullOrEmpty(login.Username) || string.IsNullOrEmpty(login.Password))
            return BadRequest("Missing username or password");

        object user = null;
        string role = "";
        string name = "";
        string userId = "";

        var isStudentId = Regex.IsMatch(login.Username, @"^\d{2}\.\d{2}\.\d{3}\.\d{3}$");        if (isStudentId)
        {
            var student = _context.Students.FirstOrDefault(s => s.StudentId == login.Username);
            if (student == null) return Unauthorized("Invalid credentials");
              // Xác thực mật khẩu bằng BCrypt
            if (!PasswordHelper.VerifyPassword(login.Password, student.Password))
                return Unauthorized("Invalid credentials");

            user = student;
            role = "Student";
            name = student.FullName;
            userId = student.StudentId;
        }
        else if (login.Username.Contains("@"))
        {
            var lecturer = _context.Lecturers.FirstOrDefault(l => l.Email == login.Username);
            if (lecturer == null) return Unauthorized("Invalid credentials");
              // Xác thực mật khẩu bằng BCrypt
            if (!PasswordHelper.VerifyPassword(login.Password, lecturer.Password))
                return Unauthorized("Invalid credentials");

            user = lecturer;
            role = "Lecturer";
            name = lecturer.FullName;
            userId = lecturer.LecturerId;
        }
        else
        {
            return BadRequest("Invalid username format");
        }        // Generate JWT
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "default-key-if-null");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role, role),
            }),
            Expires = DateTime.UtcNow.AddHours(6),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return Ok(new
        {
            Token = tokenString,
            Role = role,
            UserId = userId,
            Name = name
        });
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var name = User.FindFirstValue(ClaimTypes.Name);
        var role = User.FindFirstValue(ClaimTypes.Role);

        return Ok(new { UserId = userId, Name = name, Role = role });
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var role = User.FindFirstValue(ClaimTypes.Role);        if (role == "Student")
        {
            var student = _context.Students.FirstOrDefault(s => s.StudentId == userId);
            if (student == null)
                return BadRequest("Student not found");
              // Xác thực mật khẩu hiện tại bằng BCrypt
            if (!PasswordHelper.VerifyPassword(dto.CurrentPassword, student.Password))
                return BadRequest("Old password incorrect");

            // Mã hóa mật khẩu mới bằng BCrypt
            student.Password = PasswordHelper.HashPassword(dto.NewPassword);
            _context.Students.Update(student);
        }
        else if (role == "Lecturer")
        {
            var lecturer = _context.Lecturers.FirstOrDefault(l => l.LecturerId == userId);
            if (lecturer == null)
                return BadRequest("Lecturer not found");
              // Xác thực mật khẩu hiện tại bằng BCrypt
            if (!PasswordHelper.VerifyPassword(dto.CurrentPassword, lecturer.Password))
                return BadRequest("Old password incorrect");

            // Mã hóa mật khẩu mới bằng BCrypt
            lecturer.Password = PasswordHelper.HashPassword(dto.NewPassword);
            _context.Lecturers.Update(lecturer);
        }

        await _context.SaveChangesAsync();
        return Ok("Password changed successfully");
    }

    [HttpPost("hash-all-passwords")]
    public async Task<IActionResult> HashAllPasswords()
    {
        // Mã hóa mật khẩu cho sinh viên
        var students = _context.Students.ToList();
        foreach (var student in students)
        {            // Kiểm tra xem mật khẩu đã được mã hóa chưa
            if (!student.Password.StartsWith("$2"))
            {
                student.Password = PasswordHelper.HashPassword(student.Password);
            }
        }

        // Mã hóa mật khẩu cho giảng viên
        var lecturers = _context.Lecturers.ToList();
        foreach (var lecturer in lecturers)
        {            // Kiểm tra xem mật khẩu đã được mã hóa chưa
            if (!lecturer.Password.StartsWith("$2"))
            {
                lecturer.Password = PasswordHelper.HashPassword(lecturer.Password);
            }
        }

        await _context.SaveChangesAsync();
        return Ok("All passwords have been hashed successfully");
    }
}
