using System;
using System.Collections.Generic;

namespace KofCWSCWebsite.Models;

public partial class TblWebFileStorage
{
    public int Id { get; set; }

    public string FileName { get; set; } = null!;

    public byte[] Data { get; set; } = null!;

    public long Length { get; set; }

    public string ContentType { get; set; } = null!;
}
