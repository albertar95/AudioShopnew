//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AudioShopFrontend.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Comment
    {
        public System.Guid NidComment { get; set; }
        public System.Guid NidUser { get; set; }
        public string CommentText { get; set; }
        public Nullable<byte> State { get; set; }
        public System.DateTime CreateDate { get; set; }
    
        public virtual User User { get; set; }
    }
}
