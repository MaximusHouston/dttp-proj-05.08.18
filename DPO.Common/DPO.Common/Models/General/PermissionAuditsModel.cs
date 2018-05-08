using System;


namespace DPO.Common.Models.General
{
    public class PermissionAuditsModel
    {
        public long Id { get; set; }
        public long PermissionId { get; set; }
        public long ParentPermissionId { get; set; }
        public long ObjectId { get; set; }
        public int PermissionTypeId { get; set; }
        public int ReferenceId { get; set; }
        public int ObjectEntityId { get; set; }
        public int ReferenceEntityId { get; set; }
        public long ModifyByUserId { get; set; }
        public DateTime ModifyDate { get; set; }
        public int ModifyByUserTypeId { get; set; }
        public long EffectedUserId { get; set; }
        public int EffectedUserTypeId { get; set; }
        public string TypeOfAction { get; set; }
    }
}
