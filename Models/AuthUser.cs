using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InsuranceClaimsApi.Models;

[Table("auth_user")]
public partial class AuthUser
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("email")]
    [StringLength(255)]
    public string Email { get; set; } = null!;

    [Column("password_hash")]
    public string PasswordHash { get; set; } = null!;

    [Column("display_name")]
    [StringLength(255)]
    public string? DisplayName { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("create_dt", TypeName = "timestamp without time zone")]
    public DateTime CreateDt { get; set; }

    [Column("last_update_dt", TypeName = "timestamp without time zone")]
    public DateTime LastUpdateDt { get; set; }
}
