using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Message", Schema = "Identity")]
[Microsoft.EntityFrameworkCore.Index("ParentMessageId", Name = "IX_Message_ParentMessageId")]
[Microsoft.EntityFrameworkCore.Index("ReceiverId", Name = "IX_Message_ReceiverId")]
[Microsoft.EntityFrameworkCore.Index("SenderId", Name = "IX_Message_SenderId")]
[Microsoft.EntityFrameworkCore.Index("ThreadRootMessageId", Name = "IX_Message_ThreadRootMessageId")]
public partial class Message
{
    [Key]
    public long Id { get; set; }

    public long SenderId { get; set; }

    public long ReceiverId { get; set; }

    [StringLength(500)]
    public string Subject { get; set; } = null!;

    [StringLength(4000)]
    public string Body { get; set; } = null!;

    public bool IsRead { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ReadDate { get; set; }

    public long? ParentMessageId { get; set; }

    public long? ThreadRootMessageId { get; set; }

    [Column("title")]
    [StringLength(256)]
    public string Title { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastModifiedDate { get; set; }

    [Column("IPAddress")]
    [StringLength(128)]
    public string Ipaddress { get; set; } = null!;

    public bool IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [InverseProperty("ParentMessage")]
    public virtual ICollection<Message> InverseParentMessage { get; set; } = new List<Message>();

    [InverseProperty("ThreadRootMessage")]
    public virtual ICollection<Message> InverseThreadRootMessage { get; set; } = new List<Message>();

    [InverseProperty("Message")]
    public virtual ICollection<MessageAttachment> MessageAttachments { get; set; } = new List<MessageAttachment>();

    [ForeignKey("ParentMessageId")]
    [InverseProperty("InverseParentMessage")]
    public virtual Message? ParentMessage { get; set; }

    [ForeignKey("ReceiverId")]
    [InverseProperty("MessageReceivers")]
    public virtual AspNetUser Receiver { get; set; } = null!;

    [ForeignKey("SenderId")]
    [InverseProperty("MessageSenders")]
    public virtual AspNetUser Sender { get; set; } = null!;

    [ForeignKey("ThreadRootMessageId")]
    [InverseProperty("InverseThreadRootMessage")]
    public virtual Message? ThreadRootMessage { get; set; }
}
