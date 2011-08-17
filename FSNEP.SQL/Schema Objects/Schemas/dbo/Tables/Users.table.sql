CREATE TABLE [dbo].[Users] (
    [UserId]        UNIQUEIDENTIFIER NOT NULL,
    [UserName]      NVARCHAR (256)   NOT NULL,
    [Email]         NVARCHAR (256)   NOT NULL,
    [FirstName]     VARCHAR (50)     NOT NULL,
    [LastName]      VARCHAR (50)     NOT NULL,
    [SupervisorID]  UNIQUEIDENTIFIER NULL,
    [DelegateID]    UNIQUEIDENTIFIER NULL,
    [CreatedByID]   UNIQUEIDENTIFIER NULL,
    [FundTypeID]    INT              NULL,
    [Salary]        FLOAT            NOT NULL,
    [BenefitRate]   FLOAT            NOT NULL,
    [FTE]           FLOAT            NOT NULL,
    [ResetPassword] BIT              NULL,
    [Token]         UNIQUEIDENTIFIER NULL,
    [IsActive]      BIT              NOT NULL
);

