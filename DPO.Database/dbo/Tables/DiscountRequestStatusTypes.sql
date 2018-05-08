CREATE TABLE [dbo].[DiscountRequestStatusTypes] (
    [DiscountRequestStatusTypeId] TINYINT           NOT NULL,
    [Description]         NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_DiscountRequestStatusType] PRIMARY KEY CLUSTERED ([DiscountRequestStatusTypeId] ASC)
);

