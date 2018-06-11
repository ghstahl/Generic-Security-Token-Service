// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IdentityServer4.Test;
using QuickstartIdentityServer;

namespace IdentityServer4.Quickstart.UI
{
    public class LoginInputModel
    {
        [Required]
        public string Username { get; set; }
        private List<MultiFactorRecord> _multiFactorRecords;

        public List<MultiFactorRecord> MultiFactorRecords =>
            _multiFactorRecords = _multiFactorRecords ?? new List<MultiFactorRecord>();
        public string ReturnUrl { get; set; }
    }
}