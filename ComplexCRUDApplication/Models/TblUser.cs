using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ComplexCRUDApplication.Models;

[Table("tbl_user")]
public partial class TblUser
{
    [Key]
    [Column("code")]
    [StringLength(50)]
    [Unicode(false)]
    public string Code { get; set; } = null!;

    [Column("name")]
    [StringLength(250)]
    [Unicode(false)]
    public string? Name { get; set; }

    [Column("email")]
    [StringLength(100)]
    [Unicode(false)]
    public string? Email { get; set; }

    [Column("phone")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Phone { get; set; }

    [Column("password")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Password { get; set; }

    [Column("is_active")]
    public bool? IsActive { get; set; }

    [Column("role")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Role { get; set; }
}
