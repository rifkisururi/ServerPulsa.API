using System;
using System.ComponentModel.DataAnnotations;

public class CallbackPaymentDuitku
{
    [Key]
    public int id { get; set; }

    [Required]
    [StringLength(200)]
    public string reference { get; set; }

    [Required]
    public int amount { get; set; }

    [Required]
    public int fee { get; set; }

    [StringLength(100)]
    public string statusCode { get; set; }

    [Required]
    [StringLength(200)]
    public string statusMessage { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime created_date { get; set; }
}
