﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    [Owned]
    public class RefreshToken
    {
        public string Token { get; set; }

        public DateTime ExpiresOn { get; set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresOn;

        public DateTime CreateOn { get; set; }

        public DateTime? RevokeOn { get; set; }

        public bool IsActive => RevokeOn == null && !IsExpired;
    }
}
