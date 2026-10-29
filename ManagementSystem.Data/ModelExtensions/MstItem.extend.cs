using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ManagementSystem.Data.Models;

public partial class MstItem
{
    [NotMapped]
    public bool HasImageUpload { get; set; }
    
}
