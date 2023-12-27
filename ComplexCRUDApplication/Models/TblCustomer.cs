using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ComplexCRUDApplication.Models;

[Table("tbl_customer")]
public partial class TblCustomer
{
    [Key]
    [Column("code")]
    [StringLength(50)]
    [Unicode(false)]
    public string Code { get; set; } = null!;

    [Column("name")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Name { get; set; }

    [Column("email")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Email { get; set; }

    [Column("phone")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Phone { get; set; }

    [Column("credit_limit", TypeName = "decimal(18, 2)")]
    public decimal? CreditLimit { get; set; }

    [Column("is_active")]
    public bool? IsActive { get; set; }

    [Column("tax_code")]
    public int? TaxCode { get; set; }
}
