﻿using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;
using StellarWallet.Application.Interfaces;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Repositories;

namespace StellarWallet.Application.Services
{
    public class AuthService(IUserRepository userRepository, IJwtService jwtService, IEncryptionService encryptionService) : IAuthService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IJwtService _jwtService = jwtService;
        private readonly IEncryptionService _encryptionService = encryptionService;

        public async Task<LoggedDto> Login(LoginDto loginDto)
        {
            User? user = await _userRepository.GetBy("Email", loginDto.Email) ?? throw new Exception("User not found");
            if (!_encryptionService.Verify(loginDto.Password, user.Password))
                throw new Exception("Invalid credentials");

            string createdToken = _jwtService.CreateToken(user.Email, user.Role);

            return new LoggedDto(true, createdToken, user.PublicKey);
        }

        public bool AuthenticateEmail(string jwt, string email)
        {
            string jwtEmail = _jwtService.DecodeToken(jwt) ?? throw new Exception("Unauthorized");

            return jwtEmail.Equals(email);
        }
    }
}
