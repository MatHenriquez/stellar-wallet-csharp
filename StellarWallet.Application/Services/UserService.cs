﻿using AutoMapper;
using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;
using StellarWallet.Application.Interfaces;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Repositories;

namespace StellarWallet.Application.Services
{
    public class UserService(IUserRepository userRepository, IJwtService jwtService, IEncryptionService encryptionService, IMapper mapper) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IJwtService _jwtService = jwtService;
        private readonly IEncryptionService _encryptionService = encryptionService;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<UserDto>> GetAll()
        {
            IEnumerable<User> users = await _userRepository.GetAll();
            return _mapper.Map<UserDto[]>(users);
        }

        public async Task<UserDto> GetById(int id)
        {
            User foundUser = await _userRepository.GetById(id);
            return _mapper.Map<UserDto>(foundUser);
        }

        public async Task<LoggedDto> Add(UserCreationDto user)
        {
            User? foundUser = await _userRepository.GetBy("Email", user.Email);
            if (foundUser is not null)
                throw new Exception("User already exists");

            user.Password = _encryptionService.Encrypt(user.Password);

            await _userRepository.Add(_mapper.Map<User>(user));

            string createdToken = _jwtService.CreateToken(user.Email);

            return new LoggedDto(true, createdToken, user.PublicKey);
        }

        public async Task Update(UserUpdateDto user)
        {
            if (user.Password is not null)
                user.Password = _encryptionService.Encrypt(user.Password);

            await _userRepository.GetById(user.Id);
            await _userRepository.Update(_mapper.Map<User>(user));
        }

        public async Task Delete(int id)
        {
            await _userRepository.GetById(id);
            await _userRepository.Delete(id);
        }
    }
}
