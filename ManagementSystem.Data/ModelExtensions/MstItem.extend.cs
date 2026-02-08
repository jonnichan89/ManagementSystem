using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ManagementSystem.Data.Models;

public partial class MstItem
{
    [NotMapped]
    public string SearchKey { get { return ItemCode + ItemName; } }

    [NotMapped]
    public bool PutSearchToTop
    {
        set
        {
            Askterisk = (value ? "*" : "");
        }
    }

    [NotMapped]
    public string Askterisk { get; private set; } = "";
}
