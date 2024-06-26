﻿using System.ComponentModel.DataAnnotations;

namespace StellarWallet.Application.Dtos.Requests
{
    public class SendPaymentDto(string destinationPublicKey, decimal amount)
    {
        [Required(ErrorMessage = "Public key is required")]
        [StringLength(56, MinimumLength = 56, ErrorMessage = "Public key must have 56 characters")]
        public string DestinationPublicKey { get; set; } = destinationPublicKey;

        [Range(0.0001, double.MaxValue, ErrorMessage = "Invalid amount")]
        public decimal Amount { get; set; } = amount;
    }
}
