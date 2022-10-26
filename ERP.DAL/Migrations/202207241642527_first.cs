namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class first : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountsTrees",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AccountNumber = c.Long(nullable: false),
                        AccountName = c.String(),
                        ParentId = c.Guid(),
                        TypeId = c.Int(),
                        AccountLevel = c.Int(),
                        SelectedTree = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AccountsTrees", t => t.ParentId)
                .ForeignKey("dbo.SelectorTypes", t => t.TypeId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.ParentId)
                .Index(t => t.TypeId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.Assets",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AccountTreeId = c.Guid(),
                        AccountTreeParentId = c.Guid(),
                        AccountTreeExpenseId = c.Guid(),
                        AccountTreeDestructionId = c.Guid(),
                        BranchId = c.Guid(),
                        SafeId = c.Guid(),
                        BankAccountId = c.Guid(),
                        IsIntialBalance = c.Boolean(nullable: false),
                        IsCarStore = c.Boolean(nullable: false),
                        IsNotCarStore = c.Boolean(nullable: false),
                        Name = c.String(),
                        PurchaseDate = c.DateTime(),
                        Amount = c.Double(nullable: false),
                        IsDestruction = c.Boolean(nullable: false),
                        DestructionAmount = c.Double(nullable: false),
                        OperationDate = c.DateTime(),
                        ScrapValue = c.Double(nullable: false),
                        UsefulLife = c.Int(nullable: false),
                        Notes = c.String(),
                        Description = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BankAccounts", t => t.BankAccountId)
                .ForeignKey("dbo.Branches", t => t.BranchId)
                .ForeignKey("dbo.Safes", t => t.SafeId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .ForeignKey("dbo.AccountsTrees", t => t.AccountTreeId)
                .ForeignKey("dbo.AccountsTrees", t => t.AccountTreeDestructionId)
                .ForeignKey("dbo.AccountsTrees", t => t.AccountTreeExpenseId)
                .ForeignKey("dbo.AccountsTrees", t => t.AccountTreeParentId)
                .Index(t => t.AccountTreeId)
                .Index(t => t.AccountTreeParentId)
                .Index(t => t.AccountTreeExpenseId)
                .Index(t => t.AccountTreeDestructionId)
                .Index(t => t.BranchId)
                .Index(t => t.SafeId)
                .Index(t => t.BankAccountId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.BankAccounts",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AccountsTreeId = c.Guid(),
                        BankId = c.Guid(),
                        AccountName = c.String(),
                        AccountNo = c.String(),
                        IBAN = c.String(),
                        SwiftCode = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AccountsTrees", t => t.AccountsTreeId)
                .ForeignKey("dbo.Banks", t => t.BankId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.AccountsTreeId)
                .Index(t => t.BankId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.Banks",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CityId = c.Guid(),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cities", t => t.CityId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CityId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.Cities",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CountryId = c.Guid(),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Countries", t => t.CountryId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CountryId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.Areas",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CityId = c.Guid(),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cities", t => t.CityId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CityId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.Branches",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Tel1 = c.String(),
                        Tel2 = c.String(),
                        AreaId = c.Guid(),
                        Address = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Areas", t => t.AreaId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.AreaId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.Cheques",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        EmployeeId = c.Guid(),
                        BySaleMen = c.Boolean(nullable: false),
                        BranchId = c.Guid(),
                        SupplierId = c.Guid(),
                        CustomerId = c.Guid(),
                        BankAccountId = c.Guid(),
                        InvoiceId = c.Guid(),
                        CheckNumber = c.String(),
                        CheckDate = c.DateTime(),
                        CheckDueDate = c.DateTime(),
                        CollectionDate = c.DateTime(),
                        Amount = c.Double(nullable: false),
                        Notes = c.String(),
                        IsCollected = c.Boolean(nullable: false),
                        IsRefused = c.Boolean(nullable: false),
                        RefuseDate = c.DateTime(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BankAccounts", t => t.BankAccountId)
                .ForeignKey("dbo.Branches", t => t.BranchId)
                .ForeignKey("dbo.People", t => t.CustomerId)
                .ForeignKey("dbo.People", t => t.SupplierId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.EmployeeId)
                .Index(t => t.BranchId)
                .Index(t => t.SupplierId)
                .Index(t => t.CustomerId)
                .Index(t => t.BankAccountId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        EmpGuid = c.Guid(nullable: false),
                        PersonId = c.Guid(),
                        BranchId = c.Guid(),
                        StoreId = c.Guid(),
                        DepartmentId = c.Guid(),
                        JobId = c.Guid(),
                        IsSaleMen = c.Boolean(nullable: false),
                        NationalID = c.String(),
                        BirthDay = c.DateTime(),
                        SocialStatusId = c.Int(),
                        DateOfHiring = c.DateTime(),
                        HasRole = c.Boolean(nullable: false),
                        CommissionPercentage = c.Double(nullable: false),
                        NumberOfInsurance = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                        Store_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Branches", t => t.BranchId)
                .ForeignKey("dbo.Stores", t => t.Store_Id)
                .ForeignKey("dbo.Departments", t => t.DepartmentId)
                .ForeignKey("dbo.Jobs", t => t.JobId)
                .ForeignKey("dbo.People", t => t.PersonId)
                .ForeignKey("dbo.SocialStatus", t => t.SocialStatusId)
                .ForeignKey("dbo.Stores", t => t.StoreId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.PersonId)
                .Index(t => t.BranchId)
                .Index(t => t.StoreId)
                .Index(t => t.DepartmentId)
                .Index(t => t.JobId)
                .Index(t => t.SocialStatusId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy)
                .Index(t => t.Store_Id);
            
            CreateTable(
                "dbo.Contracts",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ConGuid = c.Guid(nullable: false),
                        ContractTypeId = c.Guid(),
                        ContractSalaryTypeId = c.Int(),
                        EmployeeId = c.Guid(),
                        FromDate = c.DateTime(),
                        ToDate = c.DateTime(),
                        NumberMonths = c.Int(nullable: false),
                        Salary = c.Double(nullable: false),
                        OverTime = c.Double(nullable: false),
                        TotalSalaryAddition = c.Double(nullable: false),
                        TotalSalaryPenalties = c.Double(nullable: false),
                        TotalDefinitionVacations = c.Double(nullable: false),
                        IsApproval = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        FinishedDate = c.DateTime(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ContractSalaryTypes", t => t.ContractSalaryTypeId)
                .ForeignKey("dbo.ContractTypes", t => t.ContractTypeId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.ContractTypeId)
                .Index(t => t.ContractSalaryTypeId)
                .Index(t => t.EmployeeId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.ContractDefinitionVacations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ContractId = c.Guid(),
                        VacationTypeId = c.Guid(),
                        DayNumber = c.Int(nullable: false),
                        Notes = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contracts", t => t.ContractId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .ForeignKey("dbo.VacationTypes", t => t.VacationTypeId)
                .Index(t => t.ContractId)
                .Index(t => t.VacationTypeId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PersonId = c.Guid(),
                        UserName = c.String(),
                        Pass = c.String(),
                        RoleId = c.Guid(),
                        IsActive = c.Boolean(nullable: false),
                        SessionKey = c.String(),
                        LastLogin = c.DateTime(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                        Person_Id = c.Guid(),
                        Role_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.People", t => t.Person_Id)
                .ForeignKey("dbo.People", t => t.PersonId)
                .ForeignKey("dbo.Roles", t => t.Role_Id)
                .ForeignKey("dbo.Roles", t => t.RoleId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.PersonId)
                .Index(t => t.RoleId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy)
                .Index(t => t.Person_Id)
                .Index(t => t.Role_Id);
            
            CreateTable(
                "dbo.People",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AccountsTreeCustomerId = c.Guid(),
                        AccountTreeSupplierId = c.Guid(),
                        AccountTreeCustomerEmpId = c.Guid(),
                        PersonTypeId = c.Int(),
                        PersonCategoryId = c.Guid(),
                        PersonGuid = c.Guid(nullable: false),
                        Name = c.String(),
                        Address = c.String(),
                        Mob1 = c.String(),
                        Mob2 = c.String(),
                        Tel = c.String(),
                        AreaId = c.Guid(),
                        ParentId = c.Guid(),
                        EntityName = c.String(),
                        GenderId = c.Int(),
                        ImageName = c.String(),
                        Notes = c.String(),
                        LocationPath = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Areas", t => t.AreaId)
                .ForeignKey("dbo.Genders", t => t.GenderId)
                .ForeignKey("dbo.PersonCategories", t => t.PersonCategoryId)
                .ForeignKey("dbo.People", t => t.ParentId)
                .ForeignKey("dbo.PersonTypes", t => t.PersonTypeId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .ForeignKey("dbo.AccountsTrees", t => t.AccountsTreeCustomerId)
                .ForeignKey("dbo.AccountsTrees", t => t.AccountTreeCustomerEmpId)
                .ForeignKey("dbo.AccountsTrees", t => t.AccountTreeSupplierId)
                .Index(t => t.AccountsTreeCustomerId)
                .Index(t => t.AccountTreeSupplierId)
                .Index(t => t.AccountTreeCustomerEmpId)
                .Index(t => t.PersonTypeId)
                .Index(t => t.PersonCategoryId)
                .Index(t => t.AreaId)
                .Index(t => t.ParentId)
                .Index(t => t.GenderId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.ContractVacations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ContractId = c.Guid(),
                        VacationTypeId = c.Guid(),
                        FromDate = c.DateTime(),
                        ToDate = c.DateTime(),
                        ApprovalBy = c.Guid(),
                        IsAccepted = c.Boolean(),
                        Reason = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contracts", t => t.ContractId)
                .ForeignKey("dbo.People", t => t.ApprovalBy)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .ForeignKey("dbo.VacationTypes", t => t.VacationTypeId)
                .Index(t => t.ContractId)
                .Index(t => t.VacationTypeId)
                .Index(t => t.ApprovalBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.VacationTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.ContractSchedulingAbsences",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ContractSchedulingId = c.Guid(),
                        VacationTypeId = c.Guid(),
                        FromDate = c.DateTime(),
                        ToDate = c.DateTime(),
                        AbsenceDayNumber = c.Double(nullable: false),
                        IsPenalty = c.Boolean(nullable: false),
                        PenaltyNumber = c.Double(nullable: false),
                        IsPayed = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ContractSchedulings", t => t.ContractSchedulingId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .ForeignKey("dbo.VacationTypes", t => t.VacationTypeId)
                .Index(t => t.ContractSchedulingId)
                .Index(t => t.VacationTypeId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.ContractSchedulings",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ContractId = c.Guid(),
                        Name = c.String(),
                        MonthYear = c.DateTime(),
                        ToDate = c.DateTime(),
                        IsApproval = c.Boolean(nullable: false),
                        IsPayed = c.Boolean(nullable: false),
                        TotalSalaryAddAllowances = c.Double(nullable: false),
                        TotalSalaryAddition = c.Double(nullable: false),
                        TotalEveryMonthSalaryPenaltie = c.Double(nullable: false),
                        TotalSalaryPenaltie = c.Double(nullable: false),
                        TotalAmountAbsences = c.Double(nullable: false),
                        LoanValue = c.Double(nullable: false),
                        Safy = c.Double(nullable: false),
                        PayedValue = c.Double(nullable: false),
                        RemindValue = c.Double(nullable: false),
                        Notes = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contracts", t => t.ContractId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.ContractId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.ContractAttendanceLeavings",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ContractSchedulingId = c.Guid(),
                        AttendanceLeavingDate = c.DateTime(nullable: false),
                        AttendanceTime = c.Time(precision: 7),
                        LeavingTime = c.Time(precision: 7),
                        Notes = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ContractSchedulings", t => t.ContractSchedulingId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.ContractSchedulingId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.ContractLoans",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ContractSchedulingId = c.Guid(),
                        BranchId = c.Guid(),
                        SafeId = c.Guid(),
                        BankAccountId = c.Guid(),
                        LoanDate = c.DateTime(),
                        Amount = c.Double(nullable: false),
                        AmountMonth = c.Double(nullable: false),
                        NumberMonths = c.Int(nullable: false),
                        Notes = c.String(),
                        IsApproval = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BankAccounts", t => t.BankAccountId)
                .ForeignKey("dbo.Branches", t => t.BranchId)
                .ForeignKey("dbo.ContractSchedulings", t => t.ContractSchedulingId)
                .ForeignKey("dbo.Safes", t => t.SafeId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.ContractSchedulingId)
                .Index(t => t.BranchId)
                .Index(t => t.SafeId)
                .Index(t => t.BankAccountId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.ContractLoanSchedulings",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ContractLoanId = c.Guid(),
                        ContractSchedulingId = c.Guid(),
                        IsPayed = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ContractLoans", t => t.ContractLoanId)
                .ForeignKey("dbo.ContractSchedulings", t => t.ContractSchedulingId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.ContractLoanId)
                .Index(t => t.ContractSchedulingId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.Safes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AccountsTreeId = c.Guid(),
                        ResoponsiblePersonId = c.Guid(),
                        BranchId = c.Guid(),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AccountsTrees", t => t.AccountsTreeId)
                .ForeignKey("dbo.Branches", t => t.BranchId)
                .ForeignKey("dbo.People", t => t.ResoponsiblePersonId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.AccountsTreeId)
                .Index(t => t.ResoponsiblePersonId)
                .Index(t => t.BranchId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.CustomerPayments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CustomerId = c.Guid(),
                        EmployeeId = c.Guid(),
                        BySaleMen = c.Boolean(nullable: false),
                        SellInvoiceId = c.Guid(),
                        BranchId = c.Guid(),
                        SafeId = c.Guid(),
                        PaymentDate = c.DateTime(),
                        Amount = c.Double(nullable: false),
                        Notes = c.String(),
                        IsApproval = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Branches", t => t.BranchId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .ForeignKey("dbo.People", t => t.CustomerId)
                .ForeignKey("dbo.Safes", t => t.SafeId)
                .ForeignKey("dbo.SellInvoices", t => t.SellInvoiceId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CustomerId)
                .Index(t => t.EmployeeId)
                .Index(t => t.SellInvoiceId)
                .Index(t => t.BranchId)
                .Index(t => t.SafeId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.SellInvoices",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        EmployeeId = c.Guid(),
                        CustomerId = c.Guid(),
                        BranchId = c.Guid(),
                        PaymentTypeId = c.Int(),
                        SafeId = c.Guid(),
                        BankAccountId = c.Guid(),
                        ShiftOffLineID = c.Guid(),
                        CaseId = c.Int(),
                        InvoiceGuid = c.Guid(nullable: false),
                        BySaleMen = c.Boolean(nullable: false),
                        InvoiceDate = c.DateTime(nullable: false),
                        TotalQuantity = c.Double(nullable: false),
                        TotalValue = c.Double(nullable: false),
                        TotalExpenses = c.Double(nullable: false),
                        PayedValue = c.Double(nullable: false),
                        RemindValue = c.Double(nullable: false),
                        SalesTax = c.Double(nullable: false),
                        ProfitTax = c.Double(nullable: false),
                        InvoiceDiscount = c.Double(nullable: false),
                        DiscountPercentage = c.Double(nullable: false),
                        TotalDiscount = c.Double(nullable: false),
                        Safy = c.Double(nullable: false),
                        IsApprovalAccountant = c.Boolean(nullable: false),
                        IsApprovalStore = c.Boolean(nullable: false),
                        IsFinalApproval = c.Boolean(nullable: false),
                        DueDate = c.DateTime(),
                        Notes = c.String(),
                        IsFullReturned = c.Boolean(),
                        InvoiceNumPaper = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BankAccounts", t => t.BankAccountId)
                .ForeignKey("dbo.Branches", t => t.BranchId)
                .ForeignKey("dbo.Cases", t => t.CaseId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .ForeignKey("dbo.PaymentTypes", t => t.PaymentTypeId)
                .ForeignKey("dbo.People", t => t.CustomerId)
                .ForeignKey("dbo.Safes", t => t.SafeId)
                .ForeignKey("dbo.ShiftsOfflines", t => t.ShiftOffLineID)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.EmployeeId)
                .Index(t => t.CustomerId)
                .Index(t => t.BranchId)
                .Index(t => t.PaymentTypeId)
                .Index(t => t.SafeId)
                .Index(t => t.BankAccountId)
                .Index(t => t.ShiftOffLineID)
                .Index(t => t.CaseId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.Cases",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.CasesPurchaseInvoiceHistories",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CaseId = c.Int(),
                        PurchaseInvoiceId = c.Guid(),
                        PurchaseBackInvoiceId = c.Guid(),
                        IsPurchaseInvoice = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cases", t => t.CaseId)
                .ForeignKey("dbo.PurchaseBackInvoices", t => t.PurchaseBackInvoiceId)
                .ForeignKey("dbo.PurchaseInvoices", t => t.PurchaseInvoiceId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CaseId)
                .Index(t => t.PurchaseInvoiceId)
                .Index(t => t.PurchaseBackInvoiceId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.PurchaseBackInvoices",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SupplierId = c.Guid(),
                        PurchaseInvoiceId = c.Guid(),
                        BranchId = c.Guid(),
                        PaymentTypeId = c.Int(),
                        SafeId = c.Guid(),
                        BankAccountId = c.Guid(),
                        CaseId = c.Int(),
                        ShiftOfflineID = c.Guid(),
                        InvoiceDate = c.DateTime(nullable: false),
                        InvoiceGuid = c.Guid(nullable: false),
                        TotalQuantity = c.Double(nullable: false),
                        TotalValue = c.Double(nullable: false),
                        TotalExpenses = c.Double(nullable: false),
                        PayedValue = c.Double(nullable: false),
                        RemindValue = c.Double(nullable: false),
                        SalesTax = c.Double(nullable: false),
                        ProfitTax = c.Double(nullable: false),
                        InvoiceDiscount = c.Double(nullable: false),
                        TotalDiscount = c.Double(nullable: false),
                        Safy = c.Double(nullable: false),
                        IsApprovalAccountant = c.Boolean(nullable: false),
                        IsApprovalStore = c.Boolean(nullable: false),
                        IsFinalApproval = c.Boolean(nullable: false),
                        DueDate = c.DateTime(),
                        Notes = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BankAccounts", t => t.BankAccountId)
                .ForeignKey("dbo.Branches", t => t.BranchId)
                .ForeignKey("dbo.Cases", t => t.CaseId)
                .ForeignKey("dbo.PaymentTypes", t => t.PaymentTypeId)
                .ForeignKey("dbo.People", t => t.SupplierId)
                .ForeignKey("dbo.PurchaseInvoices", t => t.PurchaseInvoiceId)
                .ForeignKey("dbo.Safes", t => t.SafeId)
                .ForeignKey("dbo.ShiftsOfflines", t => t.ShiftOfflineID)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.SupplierId)
                .Index(t => t.PurchaseInvoiceId)
                .Index(t => t.BranchId)
                .Index(t => t.PaymentTypeId)
                .Index(t => t.SafeId)
                .Index(t => t.BankAccountId)
                .Index(t => t.CaseId)
                .Index(t => t.ShiftOfflineID)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.PaymentTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.Maintenances",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        InvoiceGuid = c.Guid(nullable: false),
                        StoreId = c.Guid(),
                        BySaleMen = c.Boolean(nullable: false),
                        EmployeeSaleMenId = c.Guid(),
                        EmployeeResponseId = c.Guid(),
                        SellInvoiceId = c.Guid(),
                        InvoiceDate = c.DateTime(nullable: false),
                        DeliveryDate = c.DateTime(nullable: false),
                        CustomerId = c.Guid(),
                        BranchId = c.Guid(),
                        PaymentTypeId = c.Int(),
                        SafeId = c.Guid(),
                        BankAccountId = c.Guid(),
                        DiscountPercentage = c.Double(nullable: false),
                        InvoiceDiscount = c.Double(nullable: false),
                        TotalDiscount = c.Double(nullable: false),
                        TotalIncomes = c.Double(nullable: false),
                        TotalSpareParts = c.Double(nullable: false),
                        PayedValue = c.Double(nullable: false),
                        RemindValue = c.Double(nullable: false),
                        Safy = c.Double(nullable: false),
                        MaintenanceCaseId = c.Int(),
                        IsFinalApproval = c.Boolean(nullable: false),
                        HasCost = c.Boolean(nullable: false),
                        HasGuarantee = c.Boolean(nullable: false),
                        Notes = c.String(),
                        IsApprovalAccountant = c.Boolean(nullable: false),
                        IsApprovalStore = c.Boolean(nullable: false),
                        StoreReceiptId = c.Guid(),
                        ReceiptDate = c.DateTime(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BankAccounts", t => t.BankAccountId)
                .ForeignKey("dbo.Branches", t => t.BranchId)
                .ForeignKey("dbo.Stores", t => t.StoreReceiptId)
                .ForeignKey("dbo.Stores", t => t.StoreId)
                .ForeignKey("dbo.MaintenanceCas", t => t.MaintenanceCaseId)
                .ForeignKey("dbo.PaymentTypes", t => t.PaymentTypeId)
                .ForeignKey("dbo.People", t => t.CustomerId)
                .ForeignKey("dbo.Safes", t => t.SafeId)
                .ForeignKey("dbo.SellInvoices", t => t.SellInvoiceId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .ForeignKey("dbo.Employees", t => t.EmployeeResponseId)
                .ForeignKey("dbo.Employees", t => t.EmployeeSaleMenId)
                .Index(t => t.StoreId)
                .Index(t => t.EmployeeSaleMenId)
                .Index(t => t.EmployeeResponseId)
                .Index(t => t.SellInvoiceId)
                .Index(t => t.CustomerId)
                .Index(t => t.BranchId)
                .Index(t => t.PaymentTypeId)
                .Index(t => t.SafeId)
                .Index(t => t.BankAccountId)
                .Index(t => t.MaintenanceCaseId)
                .Index(t => t.StoreReceiptId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.ItemSerials",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ItemId = c.Guid(),
                        SerialNumber = c.String(),
                        SerialCaseId = c.Int(),
                        ProductionOrderId = c.Guid(),
                        IsItemIntial = c.Boolean(),
                        MaintenanceId = c.Guid(),
                        SellInvoiceId = c.Guid(),
                        CurrentStoreId = c.Guid(),
                        ExpirationDate = c.DateTime(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Items", t => t.ItemId)
                .ForeignKey("dbo.Maintenances", t => t.MaintenanceId)
                .ForeignKey("dbo.ProductionOrders", t => t.ProductionOrderId)
                .ForeignKey("dbo.SellInvoices", t => t.SellInvoiceId)
                .ForeignKey("dbo.SerialCas", t => t.SerialCaseId)
                .ForeignKey("dbo.Stores", t => t.CurrentStoreId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.ItemId)
                .Index(t => t.SerialCaseId)
                .Index(t => t.ProductionOrderId)
                .Index(t => t.MaintenanceId)
                .Index(t => t.SellInvoiceId)
                .Index(t => t.CurrentStoreId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.CasesItemSerialHistories",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SerialCaseId = c.Int(),
                        ItemSerialId = c.Guid(),
                        ReferrenceId = c.Guid(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ItemSerials", t => t.ItemSerialId)
                .ForeignKey("dbo.SerialCas", t => t.SerialCaseId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.SerialCaseId)
                .Index(t => t.ItemSerialId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.SerialCas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.Items",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        GroupBasicId = c.Guid(),
                        GroupSellId = c.Guid(),
                        ItemTypeId = c.Guid(),
                        Name = c.String(),
                        SellPrice = c.Double(nullable: false),
                        MaxPrice = c.Double(),
                        MinPrice = c.Double(),
                        ItemCode = c.Long(),
                        RequestLimit1 = c.Double(),
                        RequestLimit2 = c.Double(),
                        BarCode = c.String(),
                        TechnicalSpecifications = c.String(),
                        UnitId = c.Guid(),
                        UnitConvertFromId = c.Guid(),
                        UnitConvertFromCount = c.Double(nullable: false),
                        AvaliableToSell = c.Boolean(nullable: false),
                        CreateSerial = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Units", t => t.UnitId)
                .ForeignKey("dbo.Units", t => t.UnitConvertFromId)
                .ForeignKey("dbo.Groups", t => t.GroupBasicId)
                .ForeignKey("dbo.Groups", t => t.GroupSellId)
                .ForeignKey("dbo.ItemTypes", t => t.ItemTypeId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.GroupBasicId)
                .Index(t => t.GroupSellId)
                .Index(t => t.ItemTypeId)
                .Index(t => t.UnitId)
                .Index(t => t.UnitConvertFromId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.BalanceFirstDurations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        StoreId = c.Guid(),
                        ItemId = c.Guid(),
                        Quantity = c.Double(),
                        Price = c.Double(),
                        Amount = c.Double(),
                        Notes = c.String(),
                        IsApproval = c.Boolean(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Items", t => t.ItemId)
                .ForeignKey("dbo.Stores", t => t.StoreId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.StoreId)
                .Index(t => t.ItemId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.Stores",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AccountTreeId = c.Guid(),
                        BranchId = c.Guid(),
                        EmployeeId = c.Guid(),
                        IsDamages = c.Boolean(nullable: false),
                        Name = c.String(),
                        Address = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                        Employee_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AccountsTrees", t => t.AccountTreeId)
                .ForeignKey("dbo.Branches", t => t.BranchId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .ForeignKey("dbo.Employees", t => t.Employee_Id)
                .Index(t => t.AccountTreeId)
                .Index(t => t.BranchId)
                .Index(t => t.EmployeeId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy)
                .Index(t => t.Employee_Id);
            
            CreateTable(
                "dbo.InventoryInvoices",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        InvoiceGuid = c.Guid(nullable: false),
                        InvoiceDate = c.DateTime(nullable: false),
                        BranchId = c.Guid(),
                        StoreId = c.Guid(),
                        TotalDifferenceAmount = c.Double(nullable: false),
                        ItemCostCalculateId = c.Int(),
                        Notes = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Branches", t => t.BranchId)
                .ForeignKey("dbo.ItemCostCalculations", t => t.ItemCostCalculateId)
                .ForeignKey("dbo.Stores", t => t.StoreId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.BranchId)
                .Index(t => t.StoreId)
                .Index(t => t.ItemCostCalculateId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.InventoryInvoiceDetails",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        InventoryInvoiceId = c.Guid(),
                        ItemId = c.Guid(),
                        Balance = c.Double(nullable: false),
                        BalanceReal = c.Double(nullable: false),
                        DifferenceCount = c.Double(nullable: false),
                        DifferenceAmount = c.Double(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.InventoryInvoices", t => t.InventoryInvoiceId)
                .ForeignKey("dbo.Items", t => t.ItemId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.InventoryInvoiceId)
                .Index(t => t.ItemId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.ItemCostCalculations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.ProductionOrderDetails",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ProductionOrderId = c.Guid(),
                        ItemId = c.Guid(),
                        Quantity = c.Double(nullable: false),
                        StoreId = c.Guid(),
                        ItemCostCalculateId = c.Int(),
                        ItemCost = c.Double(nullable: false),
                        Quantitydamage = c.Double(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Items", t => t.ItemId)
                .ForeignKey("dbo.ItemCostCalculations", t => t.ItemCostCalculateId)
                .ForeignKey("dbo.ProductionOrders", t => t.ProductionOrderId)
                .ForeignKey("dbo.Stores", t => t.StoreId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.ProductionOrderId)
                .Index(t => t.ItemId)
                .Index(t => t.StoreId)
                .Index(t => t.ItemCostCalculateId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.ProductionOrders",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        OrderBarCode = c.String(),
                        OrderGuid = c.Guid(),
                        OrderColorId = c.Guid(),
                        BranchId = c.Guid(),
                        FinalItemId = c.Guid(),
                        ProductionStoreId = c.Guid(),
                        OrderQuantity = c.Double(nullable: false),
                        FinalItemCost = c.Double(nullable: false),
                        MaterialItemCost = c.Double(nullable: false),
                        DamagesCost = c.Double(nullable: false),
                        TotalExpenseCost = c.Double(nullable: false),
                        TotalCost = c.Double(nullable: false),
                        Notes = c.String(),
                        ProductionOrderDate = c.DateTime(nullable: false),
                        IsDone = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Branches", t => t.BranchId)
                .ForeignKey("dbo.Items", t => t.FinalItemId)
                .ForeignKey("dbo.ProductionOrderColors", t => t.OrderColorId)
                .ForeignKey("dbo.Stores", t => t.ProductionStoreId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.OrderColorId)
                .Index(t => t.BranchId)
                .Index(t => t.FinalItemId)
                .Index(t => t.ProductionStoreId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.ProductionOrderColors",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ColorHEX = c.String(),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.ProductionOrderExpens",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ProductionOrderId = c.Guid(),
                        ExpenseTypeAccountTreeId = c.Guid(),
                        Amount = c.Double(nullable: false),
                        Note = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AccountsTrees", t => t.ExpenseTypeAccountTreeId)
                .ForeignKey("dbo.ProductionOrders", t => t.ProductionOrderId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.ProductionOrderId)
                .Index(t => t.ExpenseTypeAccountTreeId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.ProductionOrderReceipts",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ProductionOrderId = c.Guid(),
                        FinalItemStoreId = c.Guid(),
                        ReceiptQuantity = c.Double(nullable: false),
                        ReceiptDate = c.DateTime(),
                        Notes = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Stores", t => t.FinalItemStoreId)
                .ForeignKey("dbo.ProductionOrders", t => t.ProductionOrderId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.ProductionOrderId)
                .Index(t => t.FinalItemStoreId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.StoresTransferDetails",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        StoresTransferId = c.Guid(),
                        ProductionOrderId = c.Guid(),
                        ItemSerialId = c.Guid(),
                        ItemId = c.Guid(),
                        Quantity = c.Double(nullable: false),
                        IsItemIntial = c.Boolean(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Items", t => t.ItemId)
                .ForeignKey("dbo.ItemSerials", t => t.ItemSerialId)
                .ForeignKey("dbo.ProductionOrders", t => t.ProductionOrderId)
                .ForeignKey("dbo.StoresTransfers", t => t.StoresTransferId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.StoresTransferId)
                .Index(t => t.ProductionOrderId)
                .Index(t => t.ItemSerialId)
                .Index(t => t.ItemId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.StoresTransfers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TransferGuid = c.Guid(nullable: false),
                        StoreFromId = c.Guid(),
                        StoreToId = c.Guid(),
                        ForSaleMen = c.Boolean(nullable: false),
                        EmployeeFromId = c.Guid(),
                        EmployeeToId = c.Guid(),
                        SaleMenStatus = c.Boolean(nullable: false),
                        SaleMenIsApproval = c.Boolean(nullable: false),
                        Notes = c.String(),
                        TransferDate = c.DateTime(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .ForeignKey("dbo.Stores", t => t.StoreFromId)
                .ForeignKey("dbo.Stores", t => t.StoreToId)
                .ForeignKey("dbo.Employees", t => t.EmployeeFromId)
                .ForeignKey("dbo.Employees", t => t.EmployeeToId)
                .Index(t => t.StoreFromId)
                .Index(t => t.StoreToId)
                .Index(t => t.EmployeeFromId)
                .Index(t => t.EmployeeToId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.ItemIntialBalances",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AccountTreeId = c.Guid(),
                        DateIntial = c.DateTime(),
                        ItemId = c.Guid(),
                        Quantity = c.Double(nullable: false),
                        Price = c.Double(nullable: false),
                        Amount = c.Double(nullable: false),
                        StoreId = c.Guid(),
                        IsApproval = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AccountsTrees", t => t.AccountTreeId)
                .ForeignKey("dbo.Items", t => t.ItemId)
                .ForeignKey("dbo.Stores", t => t.StoreId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.AccountTreeId)
                .Index(t => t.ItemId)
                .Index(t => t.StoreId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.MaintenanceDamages",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        MaintenanceDetailId = c.Guid(),
                        StoreId = c.Guid(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MaintenanceDetails", t => t.MaintenanceDetailId)
                .ForeignKey("dbo.Stores", t => t.StoreId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.MaintenanceDetailId)
                .Index(t => t.StoreId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.MaintenanceDamageDetails",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        MaintenanceDamageId = c.Guid(),
                        ItemId = c.Guid(),
                        Quantity = c.Double(nullable: false),
                        Amount = c.Double(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Items", t => t.ItemId)
                .ForeignKey("dbo.MaintenanceDamages", t => t.MaintenanceDamageId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.MaintenanceDamageId)
                .Index(t => t.ItemId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.MaintenanceDetails",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        RecordGuid = c.Guid(nullable: false),
                        MaintenanceId = c.Guid(),
                        ItemId = c.Guid(),
                        ItemSerialId = c.Guid(),
                        MaintenProblemTypeId = c.Guid(),
                        MaintenanceCaseId = c.Int(),
                        TotalItemDiscount = c.Double(nullable: false),
                        TotalItemIncomes = c.Double(nullable: false),
                        TotalItemSpareParts = c.Double(nullable: false),
                        ItemSafy = c.Double(nullable: false),
                        Note = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Items", t => t.ItemId)
                .ForeignKey("dbo.ItemSerials", t => t.ItemSerialId)
                .ForeignKey("dbo.Maintenances", t => t.MaintenanceId)
                .ForeignKey("dbo.MaintenanceCas", t => t.MaintenanceCaseId)
                .ForeignKey("dbo.MaintenProblemTypes", t => t.MaintenProblemTypeId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.MaintenanceId)
                .Index(t => t.ItemId)
                .Index(t => t.ItemSerialId)
                .Index(t => t.MaintenProblemTypeId)
                .Index(t => t.MaintenanceCaseId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.MaintenanceCas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ForAdmin = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.MaintenanceCaseHistories",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        MaintenanceCaseId = c.Int(),
                        MaintenanceId = c.Guid(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Maintenances", t => t.MaintenanceId)
                .ForeignKey("dbo.MaintenanceCas", t => t.MaintenanceCaseId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.MaintenanceCaseId)
                .Index(t => t.MaintenanceId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.MaintenanceIncomes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        MaintenanceDetailId = c.Guid(),
                        Name = c.String(),
                        Amount = c.Double(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MaintenanceDetails", t => t.MaintenanceDetailId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.MaintenanceDetailId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.MaintenanceSpareParts",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        MaintenanceDetailId = c.Guid(),
                        StoreId = c.Guid(),
                        ItemId = c.Guid(),
                        ItemSerialId = c.Guid(),
                        ProductionOrderId = c.Guid(),
                        Quantity = c.Double(nullable: false),
                        Price = c.Double(nullable: false),
                        Amount = c.Double(nullable: false),
                        SparePartDiscount = c.Double(nullable: false),
                        IsItemIntial = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Items", t => t.ItemId)
                .ForeignKey("dbo.ItemSerials", t => t.ItemSerialId)
                .ForeignKey("dbo.MaintenanceDetails", t => t.MaintenanceDetailId)
                .ForeignKey("dbo.ProductionOrders", t => t.ProductionOrderId)
                .ForeignKey("dbo.Stores", t => t.StoreId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.MaintenanceDetailId)
                .Index(t => t.StoreId)
                .Index(t => t.ItemId)
                .Index(t => t.ItemSerialId)
                .Index(t => t.ProductionOrderId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.MaintenProblemTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.PurchaseBackInvoicesDetails",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ContainerId = c.Guid(),
                        ItemId = c.Guid(),
                        PurchaseBackInvoiceId = c.Guid(),
                        StoreId = c.Guid(),
                        IsItemApprovalStore = c.Boolean(nullable: false),
                        Quantity = c.Double(nullable: false),
                        QuantityReal = c.Double(nullable: false),
                        Price = c.Double(nullable: false),
                        Amount = c.Double(nullable: false),
                        ItemDiscount = c.Double(nullable: false),
                        ItemEntryDate = c.DateTime(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Containers", t => t.ContainerId)
                .ForeignKey("dbo.Items", t => t.ItemId)
                .ForeignKey("dbo.PurchaseBackInvoices", t => t.PurchaseBackInvoiceId)
                .ForeignKey("dbo.Stores", t => t.StoreId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.ContainerId)
                .Index(t => t.ItemId)
                .Index(t => t.PurchaseBackInvoiceId)
                .Index(t => t.StoreId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.Containers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        ContainerNumber = c.String(),
                        ShippingCompany = c.String(),
                        ExitDate = c.DateTime(),
                        ArrivalDate = c.DateTime(),
                        ShippingPort = c.String(),
                        CompanyName = c.String(),
                        RespoName = c.String(),
                        RespoTel = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.PurchaseInvoicesDetails",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ContainerId = c.Guid(),
                        ItemId = c.Guid(),
                        PurchaseInvoiceId = c.Guid(),
                        StoreId = c.Guid(),
                        Quantity = c.Double(nullable: false),
                        QuantityReal = c.Double(nullable: false),
                        Price = c.Double(nullable: false),
                        Amount = c.Double(nullable: false),
                        ItemDiscount = c.Double(nullable: false),
                        ItemEntryDate = c.DateTime(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Containers", t => t.ContainerId)
                .ForeignKey("dbo.Items", t => t.ItemId)
                .ForeignKey("dbo.PurchaseInvoices", t => t.PurchaseInvoiceId)
                .ForeignKey("dbo.Stores", t => t.StoreId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.ContainerId)
                .Index(t => t.ItemId)
                .Index(t => t.PurchaseInvoiceId)
                .Index(t => t.StoreId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.PurchaseInvoices",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SupplierId = c.Guid(),
                        BranchId = c.Guid(),
                        PaymentTypeId = c.Int(),
                        SafeId = c.Guid(),
                        BankAccountId = c.Guid(),
                        CaseId = c.Int(),
                        ShiftOfflineID = c.Guid(),
                        InvoiceGuid = c.Guid(nullable: false),
                        InvoiceNumPaper = c.String(),
                        InvoiceDate = c.DateTime(nullable: false),
                        TotalQuantity = c.Double(nullable: false),
                        TotalValue = c.Double(nullable: false),
                        TotalExpenses = c.Double(nullable: false),
                        PayedValue = c.Double(nullable: false),
                        RemindValue = c.Double(nullable: false),
                        SalesTax = c.Double(nullable: false),
                        ProfitTax = c.Double(nullable: false),
                        InvoiceDiscount = c.Double(nullable: false),
                        TotalDiscount = c.Double(nullable: false),
                        Safy = c.Double(nullable: false),
                        IsApprovalAccountant = c.Boolean(nullable: false),
                        IsApprovalStore = c.Boolean(nullable: false),
                        IsFinalApproval = c.Boolean(nullable: false),
                        DueDate = c.DateTime(),
                        Notes = c.String(),
                        IsFullReturned = c.Boolean(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BankAccounts", t => t.BankAccountId)
                .ForeignKey("dbo.Branches", t => t.BranchId)
                .ForeignKey("dbo.Cases", t => t.CaseId)
                .ForeignKey("dbo.PaymentTypes", t => t.PaymentTypeId)
                .ForeignKey("dbo.People", t => t.SupplierId)
                .ForeignKey("dbo.Safes", t => t.SafeId)
                .ForeignKey("dbo.ShiftsOfflines", t => t.ShiftOfflineID)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.SupplierId)
                .Index(t => t.BranchId)
                .Index(t => t.PaymentTypeId)
                .Index(t => t.SafeId)
                .Index(t => t.BankAccountId)
                .Index(t => t.CaseId)
                .Index(t => t.ShiftOfflineID)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.PurchaseInvoicesExpens",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PurchaseInvoiceId = c.Guid(),
                        ExpenseTypeAccountTreeId = c.Guid(),
                        Amount = c.Double(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AccountsTrees", t => t.PurchaseInvoiceId)
                .ForeignKey("dbo.PurchaseInvoices", t => t.ExpenseTypeAccountTreeId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.PurchaseInvoiceId)
                .Index(t => t.ExpenseTypeAccountTreeId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.ShiftsOfflines",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PointOfSaleID = c.Guid(nullable: false),
                        Date = c.DateTime(nullable: false),
                        EmployeeID = c.Guid(nullable: false),
                        IsClosed = c.Boolean(nullable: false),
                        ClosedOn = c.DateTime(),
                        ClosedBy = c.Guid(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.ClosedBy)
                .ForeignKey("dbo.Employees", t => t.EmployeeID, cascadeDelete: true)
                .ForeignKey("dbo.PointOfSales", t => t.PointOfSaleID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.PointOfSaleID)
                .Index(t => t.EmployeeID)
                .Index(t => t.ClosedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.ExpenseIncomes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ExpenseIncomeTypeAccountTreeId = c.Guid(),
                        BranchId = c.Guid(),
                        SafeId = c.Guid(),
                        ShiftOffLineID = c.Guid(),
                        RowGuid = c.Guid(nullable: false),
                        IsExpense = c.Boolean(nullable: false),
                        PaymentDate = c.DateTime(),
                        Amount = c.Double(nullable: false),
                        Notes = c.String(),
                        IsApproval = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Branches", t => t.BranchId)
                .ForeignKey("dbo.AccountsTrees", t => t.ExpenseIncomeTypeAccountTreeId)
                .ForeignKey("dbo.Safes", t => t.SafeId)
                .ForeignKey("dbo.ShiftsOfflines", t => t.ShiftOffLineID)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.ExpenseIncomeTypeAccountTreeId)
                .Index(t => t.BranchId)
                .Index(t => t.SafeId)
                .Index(t => t.ShiftOffLineID)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.PointOfSales",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        BrunchId = c.Guid(nullable: false),
                        SafeID = c.Guid(nullable: false),
                        BankAccountID = c.Guid(nullable: false),
                        DefaultPricePolicyID = c.Guid(),
                        DefaultCustomerID = c.Guid(),
                        DefaultSupplierID = c.Guid(),
                        DefaultPaymentID = c.Int(),
                        CanDiscount = c.Boolean(nullable: false),
                        CanChangePrice = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BankAccounts", t => t.BankAccountID, cascadeDelete: true)
                .ForeignKey("dbo.Branches", t => t.BrunchId, cascadeDelete: true)
                .ForeignKey("dbo.PaymentTypes", t => t.DefaultPaymentID)
                .ForeignKey("dbo.PricingPolicies", t => t.DefaultPricePolicyID)
                .ForeignKey("dbo.Safes", t => t.SafeID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .ForeignKey("dbo.People", t => t.DefaultCustomerID)
                .ForeignKey("dbo.People", t => t.DefaultSupplierID)
                .Index(t => t.BrunchId)
                .Index(t => t.SafeID)
                .Index(t => t.BankAccountID)
                .Index(t => t.DefaultPricePolicyID)
                .Index(t => t.DefaultCustomerID)
                .Index(t => t.DefaultSupplierID)
                .Index(t => t.DefaultPaymentID)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.PricingPolicies",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.ItemPrices",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ItemId = c.Guid(),
                        PricingPolicyId = c.Guid(),
                        SellPrice = c.Double(),
                        CustomerId = c.Guid(),
                        UnitId = c.Guid(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Items", t => t.ItemId)
                .ForeignKey("dbo.People", t => t.CustomerId)
                .ForeignKey("dbo.PricingPolicies", t => t.PricingPolicyId)
                .ForeignKey("dbo.Units", t => t.UnitId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.ItemId)
                .Index(t => t.PricingPolicyId)
                .Index(t => t.CustomerId)
                .Index(t => t.UnitId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.Units",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.ItemUnits",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ItemId = c.Guid(),
                        UnitId = c.Guid(),
                        Quantity = c.Double(nullable: false),
                        SellPrice = c.Double(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Items", t => t.ItemId)
                .ForeignKey("dbo.Units", t => t.UnitId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.ItemId)
                .Index(t => t.UnitId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.Offers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        UnitId = c.Guid(),
                        QuantityOffer = c.Double(nullable: false),
                        IsDiscountVal = c.Boolean(nullable: false),
                        DiscountOffer = c.Double(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Units", t => t.UnitId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.UnitId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.SellBackInvoices",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        EmployeeId = c.Guid(),
                        SellInvoiceId = c.Guid(),
                        CustomerId = c.Guid(),
                        BranchId = c.Guid(),
                        PaymentTypeId = c.Int(),
                        SafeId = c.Guid(),
                        BankAccountId = c.Guid(),
                        ShiftOfflineID = c.Guid(),
                        CaseId = c.Int(),
                        InvoiceGuid = c.Guid(nullable: false),
                        BySaleMen = c.Boolean(nullable: false),
                        InvoiceDate = c.DateTime(nullable: false),
                        TotalQuantity = c.Double(nullable: false),
                        TotalValue = c.Double(nullable: false),
                        TotalExpenses = c.Double(nullable: false),
                        PayedValue = c.Double(nullable: false),
                        RemindValue = c.Double(nullable: false),
                        SalesTax = c.Double(nullable: false),
                        ProfitTax = c.Double(nullable: false),
                        InvoiceDiscount = c.Double(nullable: false),
                        DiscountPercentage = c.Double(nullable: false),
                        TotalDiscount = c.Double(nullable: false),
                        Safy = c.Double(nullable: false),
                        IsApprovalAccountant = c.Boolean(nullable: false),
                        IsApprovalStore = c.Boolean(nullable: false),
                        IsFinalApproval = c.Boolean(nullable: false),
                        DueDate = c.DateTime(),
                        Notes = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BankAccounts", t => t.BankAccountId)
                .ForeignKey("dbo.Branches", t => t.BranchId)
                .ForeignKey("dbo.Cases", t => t.CaseId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .ForeignKey("dbo.PaymentTypes", t => t.PaymentTypeId)
                .ForeignKey("dbo.People", t => t.CustomerId)
                .ForeignKey("dbo.Safes", t => t.SafeId)
                .ForeignKey("dbo.SellInvoices", t => t.SellInvoiceId)
                .ForeignKey("dbo.ShiftsOfflines", t => t.ShiftOfflineID)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.EmployeeId)
                .Index(t => t.SellInvoiceId)
                .Index(t => t.CustomerId)
                .Index(t => t.BranchId)
                .Index(t => t.PaymentTypeId)
                .Index(t => t.SafeId)
                .Index(t => t.BankAccountId)
                .Index(t => t.ShiftOfflineID)
                .Index(t => t.CaseId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.CasesSellInvoiceHistories",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CaseId = c.Int(),
                        SellInvoiceId = c.Guid(),
                        SellBackInvoiceId = c.Guid(),
                        IsSellInvoice = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cases", t => t.CaseId)
                .ForeignKey("dbo.SellBackInvoices", t => t.SellBackInvoiceId)
                .ForeignKey("dbo.SellInvoices", t => t.SellInvoiceId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CaseId)
                .Index(t => t.SellInvoiceId)
                .Index(t => t.SellBackInvoiceId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.SellBackInvoiceIncomes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SellBackInvoiceId = c.Guid(),
                        IncomeTypeAccountTreeId = c.Guid(),
                        Amount = c.Double(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AccountsTrees", t => t.IncomeTypeAccountTreeId)
                .ForeignKey("dbo.SellBackInvoices", t => t.SellBackInvoiceId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.SellBackInvoiceId)
                .Index(t => t.IncomeTypeAccountTreeId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.SellBackInvoicesDetails",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SellBackInvoiceId = c.Guid(),
                        StoreId = c.Guid(),
                        ItemId = c.Guid(),
                        ItemSerialId = c.Guid(),
                        Quantity = c.Double(nullable: false),
                        QuantityReal = c.Double(nullable: false),
                        Price = c.Double(nullable: false),
                        Amount = c.Double(nullable: false),
                        ItemDiscount = c.Double(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Items", t => t.ItemId)
                .ForeignKey("dbo.ItemSerials", t => t.ItemSerialId)
                .ForeignKey("dbo.SellBackInvoices", t => t.SellBackInvoiceId)
                .ForeignKey("dbo.Stores", t => t.StoreId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.SellBackInvoiceId)
                .Index(t => t.StoreId)
                .Index(t => t.ItemId)
                .Index(t => t.ItemSerialId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.SupplierPayments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SupplierId = c.Guid(),
                        PurchaseInvoiceId = c.Guid(),
                        BranchId = c.Guid(),
                        SafeId = c.Guid(),
                        BankAccountId = c.Guid(),
                        PaymentDate = c.DateTime(),
                        Amount = c.Double(nullable: false),
                        Notes = c.String(),
                        IsApproval = c.Boolean(nullable: false),
                        CheckNumber = c.String(),
                        CheckDate = c.DateTime(),
                        CheckDueDate = c.DateTime(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BankAccounts", t => t.BankAccountId)
                .ForeignKey("dbo.Branches", t => t.BranchId)
                .ForeignKey("dbo.People", t => t.SupplierId)
                .ForeignKey("dbo.PurchaseInvoices", t => t.PurchaseInvoiceId)
                .ForeignKey("dbo.Safes", t => t.SafeId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.SupplierId)
                .Index(t => t.PurchaseInvoiceId)
                .Index(t => t.BranchId)
                .Index(t => t.SafeId)
                .Index(t => t.BankAccountId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.SaleMenStoreHistories",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        EmployeeId = c.Guid(),
                        StoreId = c.Guid(),
                        TransferDate = c.DateTime(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .ForeignKey("dbo.Stores", t => t.StoreId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.EmployeeId)
                .Index(t => t.StoreId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.SellInvoicesDetails",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SellInvoiceId = c.Guid(),
                        StoreId = c.Guid(),
                        ItemId = c.Guid(),
                        ProductionOrderId = c.Guid(),
                        ItemSerialId = c.Guid(),
                        Quantity = c.Double(nullable: false),
                        QuantityReal = c.Double(nullable: false),
                        Price = c.Double(nullable: false),
                        Amount = c.Double(nullable: false),
                        ItemDiscount = c.Double(nullable: false),
                        IsItemIntial = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Items", t => t.ItemId)
                .ForeignKey("dbo.ItemSerials", t => t.ItemSerialId)
                .ForeignKey("dbo.ProductionOrders", t => t.ProductionOrderId)
                .ForeignKey("dbo.SellInvoices", t => t.SellInvoiceId)
                .ForeignKey("dbo.Stores", t => t.StoreId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.SellInvoiceId)
                .Index(t => t.StoreId)
                .Index(t => t.ItemId)
                .Index(t => t.ProductionOrderId)
                .Index(t => t.ItemSerialId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.StoreAdjustments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        StoreId = c.Guid(),
                        ItemId = c.Guid(),
                        Difference = c.Int(),
                        Cost = c.Double(),
                        CustomerId = c.Guid(),
                        Notes = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Items", t => t.ItemId)
                .ForeignKey("dbo.People", t => t.CustomerId)
                .ForeignKey("dbo.Stores", t => t.StoreId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.StoreId)
                .Index(t => t.ItemId)
                .Index(t => t.CustomerId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        GroupTypeId = c.Int(),
                        ParentId = c.Guid(),
                        Name = c.String(),
                        GroupCode = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GroupTypes", t => t.GroupTypeId)
                .ForeignKey("dbo.Groups", t => t.ParentId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.GroupTypeId)
                .Index(t => t.ParentId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.GroupTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.ItemProductionDetails",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ItemProductionId = c.Guid(),
                        ItemId = c.Guid(),
                        Quantity = c.Double(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Items", t => t.ItemId)
                .ForeignKey("dbo.ItemProductions", t => t.ItemProductionId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.ItemProductionId)
                .Index(t => t.ItemId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.ItemProductions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ItemFinalId = c.Guid(),
                        Name = c.String(),
                        CustomerId = c.Guid(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Items", t => t.ItemFinalId)
                .ForeignKey("dbo.People", t => t.CustomerId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.ItemFinalId)
                .Index(t => t.CustomerId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.ItemTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.PriceInvoicesDetails",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PriceInvoiceId = c.Guid(),
                        ItemId = c.Guid(),
                        Quantity = c.Double(nullable: false),
                        Price = c.Double(nullable: false),
                        Amount = c.Double(nullable: false),
                        ItemDiscount = c.Double(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Items", t => t.ItemId)
                .ForeignKey("dbo.PriceInvoices", t => t.PriceInvoiceId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.PriceInvoiceId)
                .Index(t => t.ItemId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.PriceInvoices",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CustomerId = c.Guid(),
                        BranchId = c.Guid(),
                        InvoiceGuid = c.Guid(nullable: false),
                        InvoiceDate = c.DateTime(nullable: false),
                        TotalQuantity = c.Double(nullable: false),
                        TotalValue = c.Double(nullable: false),
                        SalesTax = c.Double(nullable: false),
                        ProfitTax = c.Double(nullable: false),
                        InvoiceDiscount = c.Double(nullable: false),
                        DiscountPercentage = c.Double(nullable: false),
                        TotalDiscount = c.Double(nullable: false),
                        Safy = c.Double(nullable: false),
                        Notes = c.String(),
                        IsFullReturned = c.Boolean(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Branches", t => t.BranchId)
                .ForeignKey("dbo.People", t => t.CustomerId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CustomerId)
                .Index(t => t.BranchId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.PricesChanges",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        IsPurchasePrice = c.Boolean(nullable: false),
                        ItemId = c.Guid(),
                        Price = c.Double(),
                        Notes = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Items", t => t.ItemId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.ItemId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.PurchaseBackInvoicesExpens",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PurchaseBackInvoiceId = c.Guid(),
                        ExpenseTypeAccountsTreeID = c.Guid(),
                        Amount = c.Double(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AccountsTrees", t => t.ExpenseTypeAccountsTreeID)
                .ForeignKey("dbo.PurchaseBackInvoices", t => t.PurchaseBackInvoiceId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.PurchaseBackInvoiceId)
                .Index(t => t.ExpenseTypeAccountsTreeID)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.Installments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SellInvoiceId = c.Guid(),
                        InstallmentGuid = c.Guid(nullable: false),
                        PayedValue = c.Double(nullable: false),
                        Duration = c.Int(nullable: false),
                        RemindValue = c.Double(nullable: false),
                        ProfitValue = c.Double(nullable: false),
                        TotalValue = c.Double(nullable: false),
                        CommissionVal = c.Double(nullable: false),
                        StartDate = c.DateTime(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SellInvoices", t => t.SellInvoiceId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.SellInvoiceId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.InstallmentSchedules",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ScheduleGuid = c.Guid(nullable: false),
                        InstallmentId = c.Guid(),
                        Amount = c.Double(nullable: false),
                        InstallmentDate = c.DateTime(),
                        PaymentDate = c.DateTime(),
                        SaleMenId = c.Guid(),
                        PaidInTime = c.Boolean(nullable: false),
                        IsPayed = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Installments", t => t.InstallmentId)
                .ForeignKey("dbo.People", t => t.SaleMenId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.InstallmentId)
                .Index(t => t.SaleMenId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.SellInvoiceIncomes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SellInvoiceId = c.Guid(),
                        IncomeTypeAccountTreeId = c.Guid(),
                        Amount = c.Double(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AccountsTrees", t => t.IncomeTypeAccountTreeId)
                .ForeignKey("dbo.SellInvoices", t => t.SellInvoiceId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.SellInvoiceId)
                .Index(t => t.IncomeTypeAccountTreeId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.SellInvoicePayments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SellInvoiceId = c.Guid(),
                        PayGuid = c.Guid(nullable: false),
                        DueDate = c.DateTime(),
                        Amount = c.Double(nullable: false),
                        IsPayed = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SellInvoices", t => t.SellInvoiceId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.SellInvoiceId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.EmployeeGivingCustodies",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        EmployeeId = c.Guid(),
                        BranchId = c.Guid(),
                        SafeId = c.Guid(),
                        BankAccountId = c.Guid(),
                        CustodyDate = c.DateTime(),
                        Amount = c.Double(nullable: false),
                        Notes = c.String(),
                        IsApproval = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BankAccounts", t => t.BankAccountId)
                .ForeignKey("dbo.Branches", t => t.BranchId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .ForeignKey("dbo.Safes", t => t.SafeId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.EmployeeId)
                .Index(t => t.BranchId)
                .Index(t => t.SafeId)
                .Index(t => t.BankAccountId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.EmployeeReturnCashCustodies",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        EmployeeId = c.Guid(nullable: false),
                        BranchId = c.Guid(),
                        SafeId = c.Guid(),
                        BankAccountId = c.Guid(),
                        ReturnDate = c.DateTime(),
                        Amount = c.Double(nullable: false),
                        Notes = c.String(),
                        IsApproval = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BankAccounts", t => t.BankAccountId)
                .ForeignKey("dbo.Branches", t => t.BranchId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("dbo.Safes", t => t.SafeId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.EmployeeId)
                .Index(t => t.BranchId)
                .Index(t => t.SafeId)
                .Index(t => t.BankAccountId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.ContractSalaryAdditions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ContractId = c.Guid(),
                        ContractSchedulingId = c.Guid(),
                        SalaryAdditionId = c.Guid(),
                        Amount = c.Double(nullable: false),
                        AmountPayed = c.Double(nullable: false),
                        IsEveryMonth = c.Boolean(nullable: false),
                        IsAffactAbsence = c.Boolean(nullable: false),
                        IsFirstTime = c.Boolean(nullable: false),
                        AdditionDate = c.DateTime(),
                        IsPayed = c.Boolean(nullable: false),
                        Notes = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contracts", t => t.ContractId)
                .ForeignKey("dbo.ContractSchedulings", t => t.ContractSchedulingId)
                .ForeignKey("dbo.SalaryAdditions", t => t.SalaryAdditionId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.ContractId)
                .Index(t => t.ContractSchedulingId)
                .Index(t => t.SalaryAdditionId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.SalaryAdditions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SalaryAdditionTypeId = c.Guid(),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SalaryAdditionTypes", t => t.SalaryAdditionTypeId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.SalaryAdditionTypeId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.SalaryAdditionTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.ContractSalaryPenalties",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ContractId = c.Guid(),
                        ContractSchedulingId = c.Guid(),
                        SalaryPenaltyId = c.Guid(),
                        PenaltyDate = c.DateTime(),
                        EmployeeNotes = c.String(),
                        Amount = c.Double(nullable: false),
                        AmountPayed = c.Double(nullable: false),
                        IsEveryMonth = c.Boolean(nullable: false),
                        IsPayed = c.Boolean(nullable: false),
                        Notes = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contracts", t => t.ContractId)
                .ForeignKey("dbo.ContractSchedulings", t => t.ContractSchedulingId)
                .ForeignKey("dbo.SalaryPenalties", t => t.SalaryPenaltyId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.ContractId)
                .Index(t => t.ContractSchedulingId)
                .Index(t => t.SalaryPenaltyId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.SalaryPenalties",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SalaryPenaltyTypeId = c.Guid(),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SalaryPenaltyTypes", t => t.SalaryPenaltyTypeId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.SalaryPenaltyTypeId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.SalaryPenaltyTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.Genders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.PersonCategories",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        IsCustomer = c.Boolean(nullable: false),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.PersonTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.SaleMenAreas",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AreaId = c.Guid(),
                        PersonId = c.Guid(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Areas", t => t.AreaId)
                .ForeignKey("dbo.People", t => t.PersonId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.AreaId)
                .Index(t => t.PersonId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.SaleMenCustomers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        EmployeeId = c.Guid(),
                        CustomerId = c.Guid(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.People", t => t.CustomerId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.EmployeeId)
                .Index(t => t.CustomerId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        IsAdmin = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.PagesRoles",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        RoleId = c.Guid(),
                        PageId = c.Int(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pages", t => t.PageId)
                .ForeignKey("dbo.Roles", t => t.RoleId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.RoleId)
                .Index(t => t.PageId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.Pages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: false),
                        ParentId = c.Int(),
                        Name = c.String(),
                        Url = c.String(),
                        OtherUrls = c.String(),
                        Icon = c.String(),
                        IsPage = c.Boolean(nullable: false),
                        OrderNum = c.Int(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.ContractSalaryTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.ContractTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.Shifts",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        DepartmentId = c.Guid(),
                        EmployeeId = c.Guid(),
                        WorkPeriodId = c.Guid(),
                        IsDepartment = c.Boolean(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Departments", t => t.DepartmentId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .ForeignKey("dbo.WorkingPeriods", t => t.WorkPeriodId)
                .Index(t => t.DepartmentId)
                .Index(t => t.EmployeeId)
                .Index(t => t.WorkPeriodId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.WorkingPeriods",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        StartTime = c.Time(precision: 7),
                        EndTime = c.Time(precision: 7),
                        DelayFrom = c.Time(precision: 7),
                        AbsenceOf = c.Time(precision: 7),
                        Notes = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.VacationDays",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        DepartmentId = c.Guid(),
                        EmployeeId = c.Guid(),
                        IsWeekly = c.Boolean(),
                        DayId = c.Int(),
                        DateFrom = c.DateTime(),
                        DateTo = c.DateTime(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Departments", t => t.DepartmentId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .ForeignKey("dbo.WeekDays", t => t.DayId)
                .Index(t => t.DepartmentId)
                .Index(t => t.EmployeeId)
                .Index(t => t.DayId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.WeekDays",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Arrange = c.Int(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.EmployeeReturnCustodies",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        EmployeeId = c.Guid(nullable: false),
                        ExpenseTypeAccountTreeId = c.Guid(),
                        ReturnDate = c.DateTime(),
                        Amount = c.Double(nullable: false),
                        Notes = c.String(),
                        IsApproval = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("dbo.AccountsTrees", t => t.ExpenseTypeAccountTreeId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.EmployeeId)
                .Index(t => t.ExpenseTypeAccountTreeId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.Jobs",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.SocialStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.GeneralDailies",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TransactionId = c.Guid(),
                        TransactionDate = c.DateTime(),
                        AccountsTreeId = c.Guid(),
                        Debit = c.Double(nullable: false),
                        Credit = c.Double(nullable: false),
                        TransactionTypeId = c.Int(),
                        PaperNumber = c.String(),
                        POSId = c.Guid(),
                        ShiftId = c.Guid(),
                        Notes = c.String(),
                        BranchId = c.Guid(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AccountsTrees", t => t.AccountsTreeId)
                .ForeignKey("dbo.Branches", t => t.BranchId)
                .ForeignKey("dbo.TransactionsTypes", t => t.TransactionTypeId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.AccountsTreeId)
                .Index(t => t.TransactionTypeId)
                .Index(t => t.BranchId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.TransactionsTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.Vouchers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        IsVoucherPayment = c.Boolean(nullable: false),
                        BranchId = c.Guid(),
                        AccountTreeFromId = c.Guid(),
                        AccountTreeToId = c.Guid(),
                        VoucherDate = c.DateTime(),
                        Amount = c.Double(nullable: false),
                        Notes = c.String(),
                        IsApproval = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Branches", t => t.BranchId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .ForeignKey("dbo.AccountsTrees", t => t.AccountTreeFromId)
                .ForeignKey("dbo.AccountsTrees", t => t.AccountTreeToId)
                .Index(t => t.BranchId)
                .Index(t => t.AccountTreeFromId)
                .Index(t => t.AccountTreeToId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.ExpenseTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AccountsTreeId = c.Guid(),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AccountsTrees", t => t.AccountsTreeId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.AccountsTreeId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.GeneralRecords",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        BranchId = c.Guid(),
                        TransactionDate = c.DateTime(),
                        AccountTreeFromId = c.Guid(),
                        AccountTreeToId = c.Guid(),
                        Amount = c.Double(nullable: false),
                        Notes = c.String(),
                        IsApproval = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .ForeignKey("dbo.AccountsTrees", t => t.AccountTreeFromId)
                .ForeignKey("dbo.AccountsTrees", t => t.AccountTreeToId)
                .Index(t => t.AccountTreeFromId)
                .Index(t => t.AccountTreeToId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.IncomeTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AccountsTreeId = c.Guid(),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AccountsTrees", t => t.AccountsTreeId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.AccountsTreeId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.SelectorTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.ContractSchedulingsMains",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        MonthNum = c.Int(),
                        YearNum = c.Int(),
                        MonthYear = c.DateTime(),
                        TotalSalaries = c.Double(nullable: false),
                        Status = c.Int(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.DelayAbsenceSystems",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        IsDelay = c.Boolean(),
                        DelayMinNumber = c.Int(nullable: false),
                        DelayHourPenalty = c.Double(nullable: false),
                        AbsenceDayPenalty = c.Double(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.GeneralSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SName = c.String(),
                        SValue = c.String(),
                        SType = c.Int(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.GeneralSettingTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        NotificationTypeId = c.Int(),
                        Name = c.String(),
                        DueDate = c.DateTime(),
                        Amount = c.Double(nullable: false),
                        IsClosed = c.Boolean(nullable: false),
                        IsPeriodic = c.Boolean(nullable: false),
                        RefNumber = c.Guid(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.NotificationTypes", t => t.NotificationTypeId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.NotificationTypeId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.NotificationTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.UploadCenters",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        FileName = c.String(),
                        Name = c.String(),
                        ParentId = c.Guid(),
                        IsFolder = c.Boolean(nullable: false),
                        ReferenceGuid = c.Guid(),
                        UploadCenterTypeId = c.Int(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UploadCenters", t => t.ParentId)
                .ForeignKey("dbo.UploadCenterTypes", t => t.UploadCenterTypeId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.ParentId)
                .Index(t => t.UploadCenterTypeId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.UploadCenterTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UploadCenters", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.UploadCenters", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.UploadCenters", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.UploadCenters", "UploadCenterTypeId", "dbo.UploadCenterTypes");
            DropForeignKey("dbo.UploadCenterTypes", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.UploadCenterTypes", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.UploadCenterTypes", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.UploadCenters", "ParentId", "dbo.UploadCenters");
            DropForeignKey("dbo.Notifications", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.Notifications", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.Notifications", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.Notifications", "NotificationTypeId", "dbo.NotificationTypes");
            DropForeignKey("dbo.NotificationTypes", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.NotificationTypes", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.NotificationTypes", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.GeneralSettingTypes", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.GeneralSettingTypes", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.GeneralSettingTypes", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.GeneralSettings", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.GeneralSettings", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.GeneralSettings", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.DelayAbsenceSystems", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.DelayAbsenceSystems", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.DelayAbsenceSystems", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ContractSchedulingsMains", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.ContractSchedulingsMains", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ContractSchedulingsMains", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.Vouchers", "AccountTreeToId", "dbo.AccountsTrees");
            DropForeignKey("dbo.Vouchers", "AccountTreeFromId", "dbo.AccountsTrees");
            DropForeignKey("dbo.AccountsTrees", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.AccountsTrees", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.AccountsTrees", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.AccountsTrees", "TypeId", "dbo.SelectorTypes");
            DropForeignKey("dbo.SelectorTypes", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.SelectorTypes", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.SelectorTypes", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.People", "AccountTreeSupplierId", "dbo.AccountsTrees");
            DropForeignKey("dbo.People", "AccountTreeCustomerEmpId", "dbo.AccountsTrees");
            DropForeignKey("dbo.People", "AccountsTreeCustomerId", "dbo.AccountsTrees");
            DropForeignKey("dbo.IncomeTypes", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.IncomeTypes", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.IncomeTypes", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.IncomeTypes", "AccountsTreeId", "dbo.AccountsTrees");
            DropForeignKey("dbo.GeneralRecords", "AccountTreeToId", "dbo.AccountsTrees");
            DropForeignKey("dbo.GeneralRecords", "AccountTreeFromId", "dbo.AccountsTrees");
            DropForeignKey("dbo.GeneralRecords", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.GeneralRecords", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.GeneralRecords", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ExpenseTypes", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.ExpenseTypes", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ExpenseTypes", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ExpenseTypes", "AccountsTreeId", "dbo.AccountsTrees");
            DropForeignKey("dbo.Assets", "AccountTreeParentId", "dbo.AccountsTrees");
            DropForeignKey("dbo.Assets", "AccountTreeExpenseId", "dbo.AccountsTrees");
            DropForeignKey("dbo.Assets", "AccountTreeDestructionId", "dbo.AccountsTrees");
            DropForeignKey("dbo.Assets", "AccountTreeId", "dbo.AccountsTrees");
            DropForeignKey("dbo.Assets", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.Assets", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.Assets", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.Assets", "SafeId", "dbo.Safes");
            DropForeignKey("dbo.Assets", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.Assets", "BankAccountId", "dbo.BankAccounts");
            DropForeignKey("dbo.BankAccounts", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.BankAccounts", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.BankAccounts", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.BankAccounts", "BankId", "dbo.Banks");
            DropForeignKey("dbo.Banks", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.Banks", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.Banks", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.Banks", "CityId", "dbo.Cities");
            DropForeignKey("dbo.Cities", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.Cities", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.Cities", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.Cities", "CountryId", "dbo.Countries");
            DropForeignKey("dbo.Countries", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.Countries", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.Countries", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.Areas", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.Areas", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.Areas", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.Areas", "CityId", "dbo.Cities");
            DropForeignKey("dbo.Vouchers", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.Vouchers", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.Vouchers", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.Vouchers", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.Branches", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.Branches", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.Branches", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.GeneralDailies", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.GeneralDailies", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.GeneralDailies", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.GeneralDailies", "TransactionTypeId", "dbo.TransactionsTypes");
            DropForeignKey("dbo.TransactionsTypes", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.TransactionsTypes", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.TransactionsTypes", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.GeneralDailies", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.GeneralDailies", "AccountsTreeId", "dbo.AccountsTrees");
            DropForeignKey("dbo.Cheques", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.Cheques", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.Cheques", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.Cheques", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.Employees", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.Employees", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.Employees", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.StoresTransfers", "EmployeeToId", "dbo.Employees");
            DropForeignKey("dbo.StoresTransfers", "EmployeeFromId", "dbo.Employees");
            DropForeignKey("dbo.Stores", "Employee_Id", "dbo.Employees");
            DropForeignKey("dbo.Employees", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.Employees", "SocialStatusId", "dbo.SocialStatus");
            DropForeignKey("dbo.SocialStatus", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.SocialStatus", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.SocialStatus", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.Employees", "PersonId", "dbo.People");
            DropForeignKey("dbo.Maintenances", "EmployeeSaleMenId", "dbo.Employees");
            DropForeignKey("dbo.Maintenances", "EmployeeResponseId", "dbo.Employees");
            DropForeignKey("dbo.Employees", "JobId", "dbo.Jobs");
            DropForeignKey("dbo.Jobs", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.Jobs", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.Jobs", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.EmployeeReturnCustodies", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.EmployeeReturnCustodies", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.EmployeeReturnCustodies", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.EmployeeReturnCustodies", "ExpenseTypeAccountTreeId", "dbo.AccountsTrees");
            DropForeignKey("dbo.EmployeeReturnCustodies", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.Employees", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.VacationDays", "DayId", "dbo.WeekDays");
            DropForeignKey("dbo.WeekDays", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.WeekDays", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.WeekDays", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.VacationDays", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.VacationDays", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.VacationDays", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.VacationDays", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.VacationDays", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.Departments", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.Departments", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.Departments", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.Shifts", "WorkPeriodId", "dbo.WorkingPeriods");
            DropForeignKey("dbo.WorkingPeriods", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.WorkingPeriods", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.WorkingPeriods", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.Shifts", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.Shifts", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.Shifts", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.Shifts", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.Shifts", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.Contracts", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.Contracts", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.Contracts", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.Contracts", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.Contracts", "ContractTypeId", "dbo.ContractTypes");
            DropForeignKey("dbo.ContractTypes", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.ContractTypes", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ContractTypes", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.Contracts", "ContractSalaryTypeId", "dbo.ContractSalaryTypes");
            DropForeignKey("dbo.ContractSalaryTypes", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.ContractSalaryTypes", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ContractSalaryTypes", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ContractDefinitionVacations", "VacationTypeId", "dbo.VacationTypes");
            DropForeignKey("dbo.ContractDefinitionVacations", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.ContractDefinitionVacations", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ContractDefinitionVacations", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.Users", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.Users", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.Users", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.Users", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.Users", "Role_Id", "dbo.Roles");
            DropForeignKey("dbo.Roles", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.Roles", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.Roles", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.PagesRoles", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.PagesRoles", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.PagesRoles", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.PagesRoles", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.PagesRoles", "PageId", "dbo.Pages");
            DropForeignKey("dbo.Pages", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.Pages", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.Pages", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.Users", "PersonId", "dbo.People");
            DropForeignKey("dbo.Users", "Person_Id", "dbo.People");
            DropForeignKey("dbo.People", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.People", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.People", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.SaleMenCustomers", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.SaleMenCustomers", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.SaleMenCustomers", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.SaleMenCustomers", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.SaleMenCustomers", "CustomerId", "dbo.People");
            DropForeignKey("dbo.SaleMenAreas", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.SaleMenAreas", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.SaleMenAreas", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.SaleMenAreas", "PersonId", "dbo.People");
            DropForeignKey("dbo.SaleMenAreas", "AreaId", "dbo.Areas");
            DropForeignKey("dbo.PointOfSales", "DefaultSupplierID", "dbo.People");
            DropForeignKey("dbo.PointOfSales", "DefaultCustomerID", "dbo.People");
            DropForeignKey("dbo.People", "PersonTypeId", "dbo.PersonTypes");
            DropForeignKey("dbo.PersonTypes", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.PersonTypes", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.PersonTypes", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.People", "ParentId", "dbo.People");
            DropForeignKey("dbo.People", "PersonCategoryId", "dbo.PersonCategories");
            DropForeignKey("dbo.PersonCategories", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.PersonCategories", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.PersonCategories", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.People", "GenderId", "dbo.Genders");
            DropForeignKey("dbo.Genders", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.Genders", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.Genders", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ContractVacations", "VacationTypeId", "dbo.VacationTypes");
            DropForeignKey("dbo.VacationTypes", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.VacationTypes", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.VacationTypes", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ContractSchedulingAbsences", "VacationTypeId", "dbo.VacationTypes");
            DropForeignKey("dbo.ContractSchedulingAbsences", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.ContractSchedulingAbsences", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ContractSchedulingAbsences", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ContractSchedulingAbsences", "ContractSchedulingId", "dbo.ContractSchedulings");
            DropForeignKey("dbo.ContractSchedulings", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.ContractSchedulings", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ContractSchedulings", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ContractSalaryPenalties", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.ContractSalaryPenalties", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ContractSalaryPenalties", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ContractSalaryPenalties", "SalaryPenaltyId", "dbo.SalaryPenalties");
            DropForeignKey("dbo.SalaryPenalties", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.SalaryPenalties", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.SalaryPenalties", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.SalaryPenalties", "SalaryPenaltyTypeId", "dbo.SalaryPenaltyTypes");
            DropForeignKey("dbo.SalaryPenaltyTypes", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.SalaryPenaltyTypes", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.SalaryPenaltyTypes", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ContractSalaryPenalties", "ContractSchedulingId", "dbo.ContractSchedulings");
            DropForeignKey("dbo.ContractSalaryPenalties", "ContractId", "dbo.Contracts");
            DropForeignKey("dbo.ContractSalaryAdditions", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.ContractSalaryAdditions", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ContractSalaryAdditions", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ContractSalaryAdditions", "SalaryAdditionId", "dbo.SalaryAdditions");
            DropForeignKey("dbo.SalaryAdditions", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.SalaryAdditions", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.SalaryAdditions", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.SalaryAdditions", "SalaryAdditionTypeId", "dbo.SalaryAdditionTypes");
            DropForeignKey("dbo.SalaryAdditionTypes", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.SalaryAdditionTypes", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.SalaryAdditionTypes", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ContractSalaryAdditions", "ContractSchedulingId", "dbo.ContractSchedulings");
            DropForeignKey("dbo.ContractSalaryAdditions", "ContractId", "dbo.Contracts");
            DropForeignKey("dbo.ContractLoans", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.ContractLoans", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ContractLoans", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ContractLoans", "SafeId", "dbo.Safes");
            DropForeignKey("dbo.Safes", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.Safes", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.Safes", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.Safes", "ResoponsiblePersonId", "dbo.People");
            DropForeignKey("dbo.EmployeeReturnCashCustodies", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.EmployeeReturnCashCustodies", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.EmployeeReturnCashCustodies", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.EmployeeReturnCashCustodies", "SafeId", "dbo.Safes");
            DropForeignKey("dbo.EmployeeReturnCashCustodies", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.EmployeeReturnCashCustodies", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.EmployeeReturnCashCustodies", "BankAccountId", "dbo.BankAccounts");
            DropForeignKey("dbo.EmployeeGivingCustodies", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.EmployeeGivingCustodies", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.EmployeeGivingCustodies", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.EmployeeGivingCustodies", "SafeId", "dbo.Safes");
            DropForeignKey("dbo.EmployeeGivingCustodies", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.EmployeeGivingCustodies", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.EmployeeGivingCustodies", "BankAccountId", "dbo.BankAccounts");
            DropForeignKey("dbo.CustomerPayments", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.CustomerPayments", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.CustomerPayments", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.CustomerPayments", "SellInvoiceId", "dbo.SellInvoices");
            DropForeignKey("dbo.SellInvoices", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.SellInvoices", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.SellInvoices", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.SellInvoices", "ShiftOffLineID", "dbo.ShiftsOfflines");
            DropForeignKey("dbo.SellInvoicePayments", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.SellInvoicePayments", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.SellInvoicePayments", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.SellInvoicePayments", "SellInvoiceId", "dbo.SellInvoices");
            DropForeignKey("dbo.SellInvoiceIncomes", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.SellInvoiceIncomes", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.SellInvoiceIncomes", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.SellInvoiceIncomes", "SellInvoiceId", "dbo.SellInvoices");
            DropForeignKey("dbo.SellInvoiceIncomes", "IncomeTypeAccountTreeId", "dbo.AccountsTrees");
            DropForeignKey("dbo.SellInvoices", "SafeId", "dbo.Safes");
            DropForeignKey("dbo.SellInvoices", "CustomerId", "dbo.People");
            DropForeignKey("dbo.SellInvoices", "PaymentTypeId", "dbo.PaymentTypes");
            DropForeignKey("dbo.Installments", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.Installments", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.Installments", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.Installments", "SellInvoiceId", "dbo.SellInvoices");
            DropForeignKey("dbo.InstallmentSchedules", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.InstallmentSchedules", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.InstallmentSchedules", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.InstallmentSchedules", "SaleMenId", "dbo.People");
            DropForeignKey("dbo.InstallmentSchedules", "InstallmentId", "dbo.Installments");
            DropForeignKey("dbo.SellInvoices", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.SellInvoices", "CaseId", "dbo.Cases");
            DropForeignKey("dbo.Cases", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.Cases", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.Cases", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.CasesPurchaseInvoiceHistories", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.CasesPurchaseInvoiceHistories", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.CasesPurchaseInvoiceHistories", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.CasesPurchaseInvoiceHistories", "PurchaseInvoiceId", "dbo.PurchaseInvoices");
            DropForeignKey("dbo.CasesPurchaseInvoiceHistories", "PurchaseBackInvoiceId", "dbo.PurchaseBackInvoices");
            DropForeignKey("dbo.PurchaseBackInvoices", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.PurchaseBackInvoices", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.PurchaseBackInvoices", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.PurchaseBackInvoices", "ShiftOfflineID", "dbo.ShiftsOfflines");
            DropForeignKey("dbo.PurchaseBackInvoices", "SafeId", "dbo.Safes");
            DropForeignKey("dbo.PurchaseBackInvoices", "PurchaseInvoiceId", "dbo.PurchaseInvoices");
            DropForeignKey("dbo.PurchaseBackInvoicesExpens", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.PurchaseBackInvoicesExpens", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.PurchaseBackInvoicesExpens", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.PurchaseBackInvoicesExpens", "PurchaseBackInvoiceId", "dbo.PurchaseBackInvoices");
            DropForeignKey("dbo.PurchaseBackInvoicesExpens", "ExpenseTypeAccountsTreeID", "dbo.AccountsTrees");
            DropForeignKey("dbo.PurchaseBackInvoices", "SupplierId", "dbo.People");
            DropForeignKey("dbo.PurchaseBackInvoices", "PaymentTypeId", "dbo.PaymentTypes");
            DropForeignKey("dbo.PaymentTypes", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.PaymentTypes", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.PaymentTypes", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.Maintenances", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.Maintenances", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.Maintenances", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.Maintenances", "SellInvoiceId", "dbo.SellInvoices");
            DropForeignKey("dbo.Maintenances", "SafeId", "dbo.Safes");
            DropForeignKey("dbo.Maintenances", "CustomerId", "dbo.People");
            DropForeignKey("dbo.Maintenances", "PaymentTypeId", "dbo.PaymentTypes");
            DropForeignKey("dbo.Maintenances", "MaintenanceCaseId", "dbo.MaintenanceCas");
            DropForeignKey("dbo.ItemSerials", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.ItemSerials", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ItemSerials", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ItemSerials", "CurrentStoreId", "dbo.Stores");
            DropForeignKey("dbo.ItemSerials", "SerialCaseId", "dbo.SerialCas");
            DropForeignKey("dbo.ItemSerials", "SellInvoiceId", "dbo.SellInvoices");
            DropForeignKey("dbo.ItemSerials", "ProductionOrderId", "dbo.ProductionOrders");
            DropForeignKey("dbo.ItemSerials", "MaintenanceId", "dbo.Maintenances");
            DropForeignKey("dbo.ItemSerials", "ItemId", "dbo.Items");
            DropForeignKey("dbo.Items", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.Items", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.Items", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.PricesChanges", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.PricesChanges", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.PricesChanges", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.PricesChanges", "ItemId", "dbo.Items");
            DropForeignKey("dbo.PriceInvoicesDetails", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.PriceInvoicesDetails", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.PriceInvoicesDetails", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.PriceInvoicesDetails", "PriceInvoiceId", "dbo.PriceInvoices");
            DropForeignKey("dbo.PriceInvoices", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.PriceInvoices", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.PriceInvoices", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.PriceInvoices", "CustomerId", "dbo.People");
            DropForeignKey("dbo.PriceInvoices", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.PriceInvoicesDetails", "ItemId", "dbo.Items");
            DropForeignKey("dbo.Items", "ItemTypeId", "dbo.ItemTypes");
            DropForeignKey("dbo.ItemTypes", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.ItemTypes", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ItemTypes", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ItemProductionDetails", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.ItemProductionDetails", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ItemProductionDetails", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ItemProductionDetails", "ItemProductionId", "dbo.ItemProductions");
            DropForeignKey("dbo.ItemProductions", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.ItemProductions", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ItemProductions", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ItemProductions", "CustomerId", "dbo.People");
            DropForeignKey("dbo.ItemProductions", "ItemFinalId", "dbo.Items");
            DropForeignKey("dbo.ItemProductionDetails", "ItemId", "dbo.Items");
            DropForeignKey("dbo.Groups", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.Groups", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.Groups", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.Groups", "ParentId", "dbo.Groups");
            DropForeignKey("dbo.Items", "GroupSellId", "dbo.Groups");
            DropForeignKey("dbo.Items", "GroupBasicId", "dbo.Groups");
            DropForeignKey("dbo.Groups", "GroupTypeId", "dbo.GroupTypes");
            DropForeignKey("dbo.GroupTypes", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.GroupTypes", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.GroupTypes", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.BalanceFirstDurations", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.BalanceFirstDurations", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.BalanceFirstDurations", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.BalanceFirstDurations", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.Stores", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.Stores", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.Stores", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.StoresTransfers", "StoreToId", "dbo.Stores");
            DropForeignKey("dbo.StoresTransfers", "StoreFromId", "dbo.Stores");
            DropForeignKey("dbo.StoreAdjustments", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.StoreAdjustments", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.StoreAdjustments", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.StoreAdjustments", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.StoreAdjustments", "CustomerId", "dbo.People");
            DropForeignKey("dbo.StoreAdjustments", "ItemId", "dbo.Items");
            DropForeignKey("dbo.SellInvoicesDetails", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.SellInvoicesDetails", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.SellInvoicesDetails", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.SellInvoicesDetails", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.SellInvoicesDetails", "SellInvoiceId", "dbo.SellInvoices");
            DropForeignKey("dbo.SellInvoicesDetails", "ProductionOrderId", "dbo.ProductionOrders");
            DropForeignKey("dbo.SellInvoicesDetails", "ItemSerialId", "dbo.ItemSerials");
            DropForeignKey("dbo.SellInvoicesDetails", "ItemId", "dbo.Items");
            DropForeignKey("dbo.SaleMenStoreHistories", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.SaleMenStoreHistories", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.SaleMenStoreHistories", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.SaleMenStoreHistories", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.SaleMenStoreHistories", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.PurchaseBackInvoicesDetails", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.PurchaseBackInvoicesDetails", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.PurchaseBackInvoicesDetails", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.PurchaseBackInvoicesDetails", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.PurchaseBackInvoicesDetails", "PurchaseBackInvoiceId", "dbo.PurchaseBackInvoices");
            DropForeignKey("dbo.PurchaseBackInvoicesDetails", "ItemId", "dbo.Items");
            DropForeignKey("dbo.PurchaseBackInvoicesDetails", "ContainerId", "dbo.Containers");
            DropForeignKey("dbo.Containers", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.Containers", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.Containers", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.PurchaseInvoicesDetails", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.PurchaseInvoicesDetails", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.PurchaseInvoicesDetails", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.PurchaseInvoicesDetails", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.PurchaseInvoicesDetails", "PurchaseInvoiceId", "dbo.PurchaseInvoices");
            DropForeignKey("dbo.PurchaseInvoices", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.PurchaseInvoices", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.PurchaseInvoices", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.SupplierPayments", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.SupplierPayments", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.SupplierPayments", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.SupplierPayments", "SafeId", "dbo.Safes");
            DropForeignKey("dbo.SupplierPayments", "PurchaseInvoiceId", "dbo.PurchaseInvoices");
            DropForeignKey("dbo.SupplierPayments", "SupplierId", "dbo.People");
            DropForeignKey("dbo.SupplierPayments", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.SupplierPayments", "BankAccountId", "dbo.BankAccounts");
            DropForeignKey("dbo.PurchaseInvoices", "ShiftOfflineID", "dbo.ShiftsOfflines");
            DropForeignKey("dbo.ShiftsOfflines", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.ShiftsOfflines", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ShiftsOfflines", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.SellBackInvoices", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.SellBackInvoices", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.SellBackInvoices", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.SellBackInvoices", "ShiftOfflineID", "dbo.ShiftsOfflines");
            DropForeignKey("dbo.SellBackInvoices", "SellInvoiceId", "dbo.SellInvoices");
            DropForeignKey("dbo.SellBackInvoicesDetails", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.SellBackInvoicesDetails", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.SellBackInvoicesDetails", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.SellBackInvoicesDetails", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.SellBackInvoicesDetails", "SellBackInvoiceId", "dbo.SellBackInvoices");
            DropForeignKey("dbo.SellBackInvoicesDetails", "ItemSerialId", "dbo.ItemSerials");
            DropForeignKey("dbo.SellBackInvoicesDetails", "ItemId", "dbo.Items");
            DropForeignKey("dbo.SellBackInvoiceIncomes", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.SellBackInvoiceIncomes", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.SellBackInvoiceIncomes", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.SellBackInvoiceIncomes", "SellBackInvoiceId", "dbo.SellBackInvoices");
            DropForeignKey("dbo.SellBackInvoiceIncomes", "IncomeTypeAccountTreeId", "dbo.AccountsTrees");
            DropForeignKey("dbo.SellBackInvoices", "SafeId", "dbo.Safes");
            DropForeignKey("dbo.SellBackInvoices", "CustomerId", "dbo.People");
            DropForeignKey("dbo.SellBackInvoices", "PaymentTypeId", "dbo.PaymentTypes");
            DropForeignKey("dbo.SellBackInvoices", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.CasesSellInvoiceHistories", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.CasesSellInvoiceHistories", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.CasesSellInvoiceHistories", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.CasesSellInvoiceHistories", "SellInvoiceId", "dbo.SellInvoices");
            DropForeignKey("dbo.CasesSellInvoiceHistories", "SellBackInvoiceId", "dbo.SellBackInvoices");
            DropForeignKey("dbo.CasesSellInvoiceHistories", "CaseId", "dbo.Cases");
            DropForeignKey("dbo.SellBackInvoices", "CaseId", "dbo.Cases");
            DropForeignKey("dbo.SellBackInvoices", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.SellBackInvoices", "BankAccountId", "dbo.BankAccounts");
            DropForeignKey("dbo.ShiftsOfflines", "PointOfSaleID", "dbo.PointOfSales");
            DropForeignKey("dbo.PointOfSales", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.PointOfSales", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.PointOfSales", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.PointOfSales", "SafeID", "dbo.Safes");
            DropForeignKey("dbo.PointOfSales", "DefaultPricePolicyID", "dbo.PricingPolicies");
            DropForeignKey("dbo.PricingPolicies", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.PricingPolicies", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.PricingPolicies", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ItemPrices", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.ItemPrices", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ItemPrices", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ItemPrices", "UnitId", "dbo.Units");
            DropForeignKey("dbo.Units", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.Units", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.Units", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.Offers", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.Offers", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.Offers", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.Offers", "UnitId", "dbo.Units");
            DropForeignKey("dbo.ItemUnits", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.ItemUnits", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ItemUnits", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ItemUnits", "UnitId", "dbo.Units");
            DropForeignKey("dbo.ItemUnits", "ItemId", "dbo.Items");
            DropForeignKey("dbo.Items", "UnitConvertFromId", "dbo.Units");
            DropForeignKey("dbo.Items", "UnitId", "dbo.Units");
            DropForeignKey("dbo.ItemPrices", "PricingPolicyId", "dbo.PricingPolicies");
            DropForeignKey("dbo.ItemPrices", "CustomerId", "dbo.People");
            DropForeignKey("dbo.ItemPrices", "ItemId", "dbo.Items");
            DropForeignKey("dbo.PointOfSales", "DefaultPaymentID", "dbo.PaymentTypes");
            DropForeignKey("dbo.PointOfSales", "BrunchId", "dbo.Branches");
            DropForeignKey("dbo.PointOfSales", "BankAccountID", "dbo.BankAccounts");
            DropForeignKey("dbo.ExpenseIncomes", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.ExpenseIncomes", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ExpenseIncomes", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ExpenseIncomes", "ShiftOffLineID", "dbo.ShiftsOfflines");
            DropForeignKey("dbo.ExpenseIncomes", "SafeId", "dbo.Safes");
            DropForeignKey("dbo.ExpenseIncomes", "ExpenseIncomeTypeAccountTreeId", "dbo.AccountsTrees");
            DropForeignKey("dbo.ExpenseIncomes", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.ShiftsOfflines", "EmployeeID", "dbo.Employees");
            DropForeignKey("dbo.ShiftsOfflines", "ClosedBy", "dbo.Users");
            DropForeignKey("dbo.PurchaseInvoices", "SafeId", "dbo.Safes");
            DropForeignKey("dbo.PurchaseInvoicesExpens", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.PurchaseInvoicesExpens", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.PurchaseInvoicesExpens", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.PurchaseInvoicesExpens", "ExpenseTypeAccountTreeId", "dbo.PurchaseInvoices");
            DropForeignKey("dbo.PurchaseInvoicesExpens", "PurchaseInvoiceId", "dbo.AccountsTrees");
            DropForeignKey("dbo.PurchaseInvoices", "SupplierId", "dbo.People");
            DropForeignKey("dbo.PurchaseInvoices", "PaymentTypeId", "dbo.PaymentTypes");
            DropForeignKey("dbo.PurchaseInvoices", "CaseId", "dbo.Cases");
            DropForeignKey("dbo.PurchaseInvoices", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.PurchaseInvoices", "BankAccountId", "dbo.BankAccounts");
            DropForeignKey("dbo.PurchaseInvoicesDetails", "ItemId", "dbo.Items");
            DropForeignKey("dbo.PurchaseInvoicesDetails", "ContainerId", "dbo.Containers");
            DropForeignKey("dbo.Maintenances", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.Maintenances", "StoreReceiptId", "dbo.Stores");
            DropForeignKey("dbo.MaintenanceDamages", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.MaintenanceDamages", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.MaintenanceDamages", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.MaintenanceDamages", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.MaintenanceDamages", "MaintenanceDetailId", "dbo.MaintenanceDetails");
            DropForeignKey("dbo.MaintenanceDetails", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.MaintenanceDetails", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.MaintenanceDetails", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.MaintenanceDetails", "MaintenProblemTypeId", "dbo.MaintenProblemTypes");
            DropForeignKey("dbo.MaintenProblemTypes", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.MaintenProblemTypes", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.MaintenProblemTypes", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.MaintenanceSpareParts", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.MaintenanceSpareParts", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.MaintenanceSpareParts", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.MaintenanceSpareParts", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.MaintenanceSpareParts", "ProductionOrderId", "dbo.ProductionOrders");
            DropForeignKey("dbo.MaintenanceSpareParts", "MaintenanceDetailId", "dbo.MaintenanceDetails");
            DropForeignKey("dbo.MaintenanceSpareParts", "ItemSerialId", "dbo.ItemSerials");
            DropForeignKey("dbo.MaintenanceSpareParts", "ItemId", "dbo.Items");
            DropForeignKey("dbo.MaintenanceIncomes", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.MaintenanceIncomes", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.MaintenanceIncomes", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.MaintenanceIncomes", "MaintenanceDetailId", "dbo.MaintenanceDetails");
            DropForeignKey("dbo.MaintenanceDetails", "MaintenanceCaseId", "dbo.MaintenanceCas");
            DropForeignKey("dbo.MaintenanceCas", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.MaintenanceCas", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.MaintenanceCas", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.MaintenanceCaseHistories", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.MaintenanceCaseHistories", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.MaintenanceCaseHistories", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.MaintenanceCaseHistories", "MaintenanceCaseId", "dbo.MaintenanceCas");
            DropForeignKey("dbo.MaintenanceCaseHistories", "MaintenanceId", "dbo.Maintenances");
            DropForeignKey("dbo.MaintenanceDetails", "MaintenanceId", "dbo.Maintenances");
            DropForeignKey("dbo.MaintenanceDetails", "ItemSerialId", "dbo.ItemSerials");
            DropForeignKey("dbo.MaintenanceDetails", "ItemId", "dbo.Items");
            DropForeignKey("dbo.MaintenanceDamageDetails", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.MaintenanceDamageDetails", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.MaintenanceDamageDetails", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.MaintenanceDamageDetails", "MaintenanceDamageId", "dbo.MaintenanceDamages");
            DropForeignKey("dbo.MaintenanceDamageDetails", "ItemId", "dbo.Items");
            DropForeignKey("dbo.ItemIntialBalances", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.ItemIntialBalances", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ItemIntialBalances", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ItemIntialBalances", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.ItemIntialBalances", "ItemId", "dbo.Items");
            DropForeignKey("dbo.ItemIntialBalances", "AccountTreeId", "dbo.AccountsTrees");
            DropForeignKey("dbo.InventoryInvoices", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.InventoryInvoices", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.InventoryInvoices", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.InventoryInvoices", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.InventoryInvoices", "ItemCostCalculateId", "dbo.ItemCostCalculations");
            DropForeignKey("dbo.ItemCostCalculations", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.ItemCostCalculations", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ItemCostCalculations", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ProductionOrderDetails", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.ProductionOrderDetails", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ProductionOrderDetails", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ProductionOrderDetails", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.ProductionOrderDetails", "ProductionOrderId", "dbo.ProductionOrders");
            DropForeignKey("dbo.ProductionOrders", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.ProductionOrders", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ProductionOrders", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.StoresTransferDetails", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.StoresTransferDetails", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.StoresTransferDetails", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.StoresTransferDetails", "StoresTransferId", "dbo.StoresTransfers");
            DropForeignKey("dbo.StoresTransfers", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.StoresTransfers", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.StoresTransfers", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.StoresTransferDetails", "ProductionOrderId", "dbo.ProductionOrders");
            DropForeignKey("dbo.StoresTransferDetails", "ItemSerialId", "dbo.ItemSerials");
            DropForeignKey("dbo.StoresTransferDetails", "ItemId", "dbo.Items");
            DropForeignKey("dbo.ProductionOrders", "ProductionStoreId", "dbo.Stores");
            DropForeignKey("dbo.ProductionOrderReceipts", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.ProductionOrderReceipts", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ProductionOrderReceipts", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ProductionOrderReceipts", "ProductionOrderId", "dbo.ProductionOrders");
            DropForeignKey("dbo.ProductionOrderReceipts", "FinalItemStoreId", "dbo.Stores");
            DropForeignKey("dbo.ProductionOrderExpens", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.ProductionOrderExpens", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ProductionOrderExpens", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ProductionOrderExpens", "ProductionOrderId", "dbo.ProductionOrders");
            DropForeignKey("dbo.ProductionOrderExpens", "ExpenseTypeAccountTreeId", "dbo.AccountsTrees");
            DropForeignKey("dbo.ProductionOrders", "OrderColorId", "dbo.ProductionOrderColors");
            DropForeignKey("dbo.ProductionOrderColors", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.ProductionOrderColors", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ProductionOrderColors", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ProductionOrders", "FinalItemId", "dbo.Items");
            DropForeignKey("dbo.ProductionOrders", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.ProductionOrderDetails", "ItemCostCalculateId", "dbo.ItemCostCalculations");
            DropForeignKey("dbo.ProductionOrderDetails", "ItemId", "dbo.Items");
            DropForeignKey("dbo.InventoryInvoiceDetails", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.InventoryInvoiceDetails", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.InventoryInvoiceDetails", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.InventoryInvoiceDetails", "ItemId", "dbo.Items");
            DropForeignKey("dbo.InventoryInvoiceDetails", "InventoryInvoiceId", "dbo.InventoryInvoices");
            DropForeignKey("dbo.InventoryInvoices", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.Employees", "Store_Id", "dbo.Stores");
            DropForeignKey("dbo.Stores", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.Stores", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.Stores", "AccountTreeId", "dbo.AccountsTrees");
            DropForeignKey("dbo.BalanceFirstDurations", "ItemId", "dbo.Items");
            DropForeignKey("dbo.CasesItemSerialHistories", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.CasesItemSerialHistories", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.CasesItemSerialHistories", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.CasesItemSerialHistories", "SerialCaseId", "dbo.SerialCas");
            DropForeignKey("dbo.SerialCas", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.SerialCas", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.SerialCas", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.CasesItemSerialHistories", "ItemSerialId", "dbo.ItemSerials");
            DropForeignKey("dbo.Maintenances", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.Maintenances", "BankAccountId", "dbo.BankAccounts");
            DropForeignKey("dbo.PurchaseBackInvoices", "CaseId", "dbo.Cases");
            DropForeignKey("dbo.PurchaseBackInvoices", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.PurchaseBackInvoices", "BankAccountId", "dbo.BankAccounts");
            DropForeignKey("dbo.CasesPurchaseInvoiceHistories", "CaseId", "dbo.Cases");
            DropForeignKey("dbo.SellInvoices", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.SellInvoices", "BankAccountId", "dbo.BankAccounts");
            DropForeignKey("dbo.CustomerPayments", "SafeId", "dbo.Safes");
            DropForeignKey("dbo.CustomerPayments", "CustomerId", "dbo.People");
            DropForeignKey("dbo.CustomerPayments", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.CustomerPayments", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.Safes", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.Safes", "AccountsTreeId", "dbo.AccountsTrees");
            DropForeignKey("dbo.ContractLoans", "ContractSchedulingId", "dbo.ContractSchedulings");
            DropForeignKey("dbo.ContractLoanSchedulings", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.ContractLoanSchedulings", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ContractLoanSchedulings", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ContractLoanSchedulings", "ContractSchedulingId", "dbo.ContractSchedulings");
            DropForeignKey("dbo.ContractLoanSchedulings", "ContractLoanId", "dbo.ContractLoans");
            DropForeignKey("dbo.ContractLoans", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.ContractLoans", "BankAccountId", "dbo.BankAccounts");
            DropForeignKey("dbo.ContractAttendanceLeavings", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.ContractAttendanceLeavings", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ContractAttendanceLeavings", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ContractAttendanceLeavings", "ContractSchedulingId", "dbo.ContractSchedulings");
            DropForeignKey("dbo.ContractSchedulings", "ContractId", "dbo.Contracts");
            DropForeignKey("dbo.ContractVacations", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.ContractVacations", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ContractVacations", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ContractVacations", "ApprovalBy", "dbo.People");
            DropForeignKey("dbo.ContractVacations", "ContractId", "dbo.Contracts");
            DropForeignKey("dbo.Cheques", "SupplierId", "dbo.People");
            DropForeignKey("dbo.Cheques", "CustomerId", "dbo.People");
            DropForeignKey("dbo.People", "AreaId", "dbo.Areas");
            DropForeignKey("dbo.ContractDefinitionVacations", "ContractId", "dbo.Contracts");
            DropForeignKey("dbo.Employees", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.Cheques", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.Cheques", "BankAccountId", "dbo.BankAccounts");
            DropForeignKey("dbo.Branches", "AreaId", "dbo.Areas");
            DropForeignKey("dbo.BankAccounts", "AccountsTreeId", "dbo.AccountsTrees");
            DropForeignKey("dbo.AccountsTrees", "ParentId", "dbo.AccountsTrees");
            DropIndex("dbo.UploadCenterTypes", new[] { "DeletedBy" });
            DropIndex("dbo.UploadCenterTypes", new[] { "ModifiedBy" });
            DropIndex("dbo.UploadCenterTypes", new[] { "CreatedBy" });
            DropIndex("dbo.UploadCenters", new[] { "DeletedBy" });
            DropIndex("dbo.UploadCenters", new[] { "ModifiedBy" });
            DropIndex("dbo.UploadCenters", new[] { "CreatedBy" });
            DropIndex("dbo.UploadCenters", new[] { "UploadCenterTypeId" });
            DropIndex("dbo.UploadCenters", new[] { "ParentId" });
            DropIndex("dbo.NotificationTypes", new[] { "DeletedBy" });
            DropIndex("dbo.NotificationTypes", new[] { "ModifiedBy" });
            DropIndex("dbo.NotificationTypes", new[] { "CreatedBy" });
            DropIndex("dbo.Notifications", new[] { "DeletedBy" });
            DropIndex("dbo.Notifications", new[] { "ModifiedBy" });
            DropIndex("dbo.Notifications", new[] { "CreatedBy" });
            DropIndex("dbo.Notifications", new[] { "NotificationTypeId" });
            DropIndex("dbo.GeneralSettingTypes", new[] { "DeletedBy" });
            DropIndex("dbo.GeneralSettingTypes", new[] { "ModifiedBy" });
            DropIndex("dbo.GeneralSettingTypes", new[] { "CreatedBy" });
            DropIndex("dbo.GeneralSettings", new[] { "DeletedBy" });
            DropIndex("dbo.GeneralSettings", new[] { "ModifiedBy" });
            DropIndex("dbo.GeneralSettings", new[] { "CreatedBy" });
            DropIndex("dbo.DelayAbsenceSystems", new[] { "DeletedBy" });
            DropIndex("dbo.DelayAbsenceSystems", new[] { "ModifiedBy" });
            DropIndex("dbo.DelayAbsenceSystems", new[] { "CreatedBy" });
            DropIndex("dbo.ContractSchedulingsMains", new[] { "DeletedBy" });
            DropIndex("dbo.ContractSchedulingsMains", new[] { "ModifiedBy" });
            DropIndex("dbo.ContractSchedulingsMains", new[] { "CreatedBy" });
            DropIndex("dbo.SelectorTypes", new[] { "DeletedBy" });
            DropIndex("dbo.SelectorTypes", new[] { "ModifiedBy" });
            DropIndex("dbo.SelectorTypes", new[] { "CreatedBy" });
            DropIndex("dbo.IncomeTypes", new[] { "DeletedBy" });
            DropIndex("dbo.IncomeTypes", new[] { "ModifiedBy" });
            DropIndex("dbo.IncomeTypes", new[] { "CreatedBy" });
            DropIndex("dbo.IncomeTypes", new[] { "AccountsTreeId" });
            DropIndex("dbo.GeneralRecords", new[] { "DeletedBy" });
            DropIndex("dbo.GeneralRecords", new[] { "ModifiedBy" });
            DropIndex("dbo.GeneralRecords", new[] { "CreatedBy" });
            DropIndex("dbo.GeneralRecords", new[] { "AccountTreeToId" });
            DropIndex("dbo.GeneralRecords", new[] { "AccountTreeFromId" });
            DropIndex("dbo.ExpenseTypes", new[] { "DeletedBy" });
            DropIndex("dbo.ExpenseTypes", new[] { "ModifiedBy" });
            DropIndex("dbo.ExpenseTypes", new[] { "CreatedBy" });
            DropIndex("dbo.ExpenseTypes", new[] { "AccountsTreeId" });
            DropIndex("dbo.Countries", new[] { "DeletedBy" });
            DropIndex("dbo.Countries", new[] { "ModifiedBy" });
            DropIndex("dbo.Countries", new[] { "CreatedBy" });
            DropIndex("dbo.Vouchers", new[] { "DeletedBy" });
            DropIndex("dbo.Vouchers", new[] { "ModifiedBy" });
            DropIndex("dbo.Vouchers", new[] { "CreatedBy" });
            DropIndex("dbo.Vouchers", new[] { "AccountTreeToId" });
            DropIndex("dbo.Vouchers", new[] { "AccountTreeFromId" });
            DropIndex("dbo.Vouchers", new[] { "BranchId" });
            DropIndex("dbo.TransactionsTypes", new[] { "DeletedBy" });
            DropIndex("dbo.TransactionsTypes", new[] { "ModifiedBy" });
            DropIndex("dbo.TransactionsTypes", new[] { "CreatedBy" });
            DropIndex("dbo.GeneralDailies", new[] { "DeletedBy" });
            DropIndex("dbo.GeneralDailies", new[] { "ModifiedBy" });
            DropIndex("dbo.GeneralDailies", new[] { "CreatedBy" });
            DropIndex("dbo.GeneralDailies", new[] { "BranchId" });
            DropIndex("dbo.GeneralDailies", new[] { "TransactionTypeId" });
            DropIndex("dbo.GeneralDailies", new[] { "AccountsTreeId" });
            DropIndex("dbo.SocialStatus", new[] { "DeletedBy" });
            DropIndex("dbo.SocialStatus", new[] { "ModifiedBy" });
            DropIndex("dbo.SocialStatus", new[] { "CreatedBy" });
            DropIndex("dbo.Jobs", new[] { "DeletedBy" });
            DropIndex("dbo.Jobs", new[] { "ModifiedBy" });
            DropIndex("dbo.Jobs", new[] { "CreatedBy" });
            DropIndex("dbo.EmployeeReturnCustodies", new[] { "DeletedBy" });
            DropIndex("dbo.EmployeeReturnCustodies", new[] { "ModifiedBy" });
            DropIndex("dbo.EmployeeReturnCustodies", new[] { "CreatedBy" });
            DropIndex("dbo.EmployeeReturnCustodies", new[] { "ExpenseTypeAccountTreeId" });
            DropIndex("dbo.EmployeeReturnCustodies", new[] { "EmployeeId" });
            DropIndex("dbo.WeekDays", new[] { "DeletedBy" });
            DropIndex("dbo.WeekDays", new[] { "ModifiedBy" });
            DropIndex("dbo.WeekDays", new[] { "CreatedBy" });
            DropIndex("dbo.VacationDays", new[] { "DeletedBy" });
            DropIndex("dbo.VacationDays", new[] { "ModifiedBy" });
            DropIndex("dbo.VacationDays", new[] { "CreatedBy" });
            DropIndex("dbo.VacationDays", new[] { "DayId" });
            DropIndex("dbo.VacationDays", new[] { "EmployeeId" });
            DropIndex("dbo.VacationDays", new[] { "DepartmentId" });
            DropIndex("dbo.WorkingPeriods", new[] { "DeletedBy" });
            DropIndex("dbo.WorkingPeriods", new[] { "ModifiedBy" });
            DropIndex("dbo.WorkingPeriods", new[] { "CreatedBy" });
            DropIndex("dbo.Shifts", new[] { "DeletedBy" });
            DropIndex("dbo.Shifts", new[] { "ModifiedBy" });
            DropIndex("dbo.Shifts", new[] { "CreatedBy" });
            DropIndex("dbo.Shifts", new[] { "WorkPeriodId" });
            DropIndex("dbo.Shifts", new[] { "EmployeeId" });
            DropIndex("dbo.Shifts", new[] { "DepartmentId" });
            DropIndex("dbo.Departments", new[] { "DeletedBy" });
            DropIndex("dbo.Departments", new[] { "ModifiedBy" });
            DropIndex("dbo.Departments", new[] { "CreatedBy" });
            DropIndex("dbo.ContractTypes", new[] { "DeletedBy" });
            DropIndex("dbo.ContractTypes", new[] { "ModifiedBy" });
            DropIndex("dbo.ContractTypes", new[] { "CreatedBy" });
            DropIndex("dbo.ContractSalaryTypes", new[] { "DeletedBy" });
            DropIndex("dbo.ContractSalaryTypes", new[] { "ModifiedBy" });
            DropIndex("dbo.ContractSalaryTypes", new[] { "CreatedBy" });
            DropIndex("dbo.Pages", new[] { "DeletedBy" });
            DropIndex("dbo.Pages", new[] { "ModifiedBy" });
            DropIndex("dbo.Pages", new[] { "CreatedBy" });
            DropIndex("dbo.PagesRoles", new[] { "DeletedBy" });
            DropIndex("dbo.PagesRoles", new[] { "ModifiedBy" });
            DropIndex("dbo.PagesRoles", new[] { "CreatedBy" });
            DropIndex("dbo.PagesRoles", new[] { "PageId" });
            DropIndex("dbo.PagesRoles", new[] { "RoleId" });
            DropIndex("dbo.Roles", new[] { "DeletedBy" });
            DropIndex("dbo.Roles", new[] { "ModifiedBy" });
            DropIndex("dbo.Roles", new[] { "CreatedBy" });
            DropIndex("dbo.SaleMenCustomers", new[] { "DeletedBy" });
            DropIndex("dbo.SaleMenCustomers", new[] { "ModifiedBy" });
            DropIndex("dbo.SaleMenCustomers", new[] { "CreatedBy" });
            DropIndex("dbo.SaleMenCustomers", new[] { "CustomerId" });
            DropIndex("dbo.SaleMenCustomers", new[] { "EmployeeId" });
            DropIndex("dbo.SaleMenAreas", new[] { "DeletedBy" });
            DropIndex("dbo.SaleMenAreas", new[] { "ModifiedBy" });
            DropIndex("dbo.SaleMenAreas", new[] { "CreatedBy" });
            DropIndex("dbo.SaleMenAreas", new[] { "PersonId" });
            DropIndex("dbo.SaleMenAreas", new[] { "AreaId" });
            DropIndex("dbo.PersonTypes", new[] { "DeletedBy" });
            DropIndex("dbo.PersonTypes", new[] { "ModifiedBy" });
            DropIndex("dbo.PersonTypes", new[] { "CreatedBy" });
            DropIndex("dbo.PersonCategories", new[] { "DeletedBy" });
            DropIndex("dbo.PersonCategories", new[] { "ModifiedBy" });
            DropIndex("dbo.PersonCategories", new[] { "CreatedBy" });
            DropIndex("dbo.Genders", new[] { "DeletedBy" });
            DropIndex("dbo.Genders", new[] { "ModifiedBy" });
            DropIndex("dbo.Genders", new[] { "CreatedBy" });
            DropIndex("dbo.SalaryPenaltyTypes", new[] { "DeletedBy" });
            DropIndex("dbo.SalaryPenaltyTypes", new[] { "ModifiedBy" });
            DropIndex("dbo.SalaryPenaltyTypes", new[] { "CreatedBy" });
            DropIndex("dbo.SalaryPenalties", new[] { "DeletedBy" });
            DropIndex("dbo.SalaryPenalties", new[] { "ModifiedBy" });
            DropIndex("dbo.SalaryPenalties", new[] { "CreatedBy" });
            DropIndex("dbo.SalaryPenalties", new[] { "SalaryPenaltyTypeId" });
            DropIndex("dbo.ContractSalaryPenalties", new[] { "DeletedBy" });
            DropIndex("dbo.ContractSalaryPenalties", new[] { "ModifiedBy" });
            DropIndex("dbo.ContractSalaryPenalties", new[] { "CreatedBy" });
            DropIndex("dbo.ContractSalaryPenalties", new[] { "SalaryPenaltyId" });
            DropIndex("dbo.ContractSalaryPenalties", new[] { "ContractSchedulingId" });
            DropIndex("dbo.ContractSalaryPenalties", new[] { "ContractId" });
            DropIndex("dbo.SalaryAdditionTypes", new[] { "DeletedBy" });
            DropIndex("dbo.SalaryAdditionTypes", new[] { "ModifiedBy" });
            DropIndex("dbo.SalaryAdditionTypes", new[] { "CreatedBy" });
            DropIndex("dbo.SalaryAdditions", new[] { "DeletedBy" });
            DropIndex("dbo.SalaryAdditions", new[] { "ModifiedBy" });
            DropIndex("dbo.SalaryAdditions", new[] { "CreatedBy" });
            DropIndex("dbo.SalaryAdditions", new[] { "SalaryAdditionTypeId" });
            DropIndex("dbo.ContractSalaryAdditions", new[] { "DeletedBy" });
            DropIndex("dbo.ContractSalaryAdditions", new[] { "ModifiedBy" });
            DropIndex("dbo.ContractSalaryAdditions", new[] { "CreatedBy" });
            DropIndex("dbo.ContractSalaryAdditions", new[] { "SalaryAdditionId" });
            DropIndex("dbo.ContractSalaryAdditions", new[] { "ContractSchedulingId" });
            DropIndex("dbo.ContractSalaryAdditions", new[] { "ContractId" });
            DropIndex("dbo.EmployeeReturnCashCustodies", new[] { "DeletedBy" });
            DropIndex("dbo.EmployeeReturnCashCustodies", new[] { "ModifiedBy" });
            DropIndex("dbo.EmployeeReturnCashCustodies", new[] { "CreatedBy" });
            DropIndex("dbo.EmployeeReturnCashCustodies", new[] { "BankAccountId" });
            DropIndex("dbo.EmployeeReturnCashCustodies", new[] { "SafeId" });
            DropIndex("dbo.EmployeeReturnCashCustodies", new[] { "BranchId" });
            DropIndex("dbo.EmployeeReturnCashCustodies", new[] { "EmployeeId" });
            DropIndex("dbo.EmployeeGivingCustodies", new[] { "DeletedBy" });
            DropIndex("dbo.EmployeeGivingCustodies", new[] { "ModifiedBy" });
            DropIndex("dbo.EmployeeGivingCustodies", new[] { "CreatedBy" });
            DropIndex("dbo.EmployeeGivingCustodies", new[] { "BankAccountId" });
            DropIndex("dbo.EmployeeGivingCustodies", new[] { "SafeId" });
            DropIndex("dbo.EmployeeGivingCustodies", new[] { "BranchId" });
            DropIndex("dbo.EmployeeGivingCustodies", new[] { "EmployeeId" });
            DropIndex("dbo.SellInvoicePayments", new[] { "DeletedBy" });
            DropIndex("dbo.SellInvoicePayments", new[] { "ModifiedBy" });
            DropIndex("dbo.SellInvoicePayments", new[] { "CreatedBy" });
            DropIndex("dbo.SellInvoicePayments", new[] { "SellInvoiceId" });
            DropIndex("dbo.SellInvoiceIncomes", new[] { "DeletedBy" });
            DropIndex("dbo.SellInvoiceIncomes", new[] { "ModifiedBy" });
            DropIndex("dbo.SellInvoiceIncomes", new[] { "CreatedBy" });
            DropIndex("dbo.SellInvoiceIncomes", new[] { "IncomeTypeAccountTreeId" });
            DropIndex("dbo.SellInvoiceIncomes", new[] { "SellInvoiceId" });
            DropIndex("dbo.InstallmentSchedules", new[] { "DeletedBy" });
            DropIndex("dbo.InstallmentSchedules", new[] { "ModifiedBy" });
            DropIndex("dbo.InstallmentSchedules", new[] { "CreatedBy" });
            DropIndex("dbo.InstallmentSchedules", new[] { "SaleMenId" });
            DropIndex("dbo.InstallmentSchedules", new[] { "InstallmentId" });
            DropIndex("dbo.Installments", new[] { "DeletedBy" });
            DropIndex("dbo.Installments", new[] { "ModifiedBy" });
            DropIndex("dbo.Installments", new[] { "CreatedBy" });
            DropIndex("dbo.Installments", new[] { "SellInvoiceId" });
            DropIndex("dbo.PurchaseBackInvoicesExpens", new[] { "DeletedBy" });
            DropIndex("dbo.PurchaseBackInvoicesExpens", new[] { "ModifiedBy" });
            DropIndex("dbo.PurchaseBackInvoicesExpens", new[] { "CreatedBy" });
            DropIndex("dbo.PurchaseBackInvoicesExpens", new[] { "ExpenseTypeAccountsTreeID" });
            DropIndex("dbo.PurchaseBackInvoicesExpens", new[] { "PurchaseBackInvoiceId" });
            DropIndex("dbo.PricesChanges", new[] { "DeletedBy" });
            DropIndex("dbo.PricesChanges", new[] { "ModifiedBy" });
            DropIndex("dbo.PricesChanges", new[] { "CreatedBy" });
            DropIndex("dbo.PricesChanges", new[] { "ItemId" });
            DropIndex("dbo.PriceInvoices", new[] { "DeletedBy" });
            DropIndex("dbo.PriceInvoices", new[] { "ModifiedBy" });
            DropIndex("dbo.PriceInvoices", new[] { "CreatedBy" });
            DropIndex("dbo.PriceInvoices", new[] { "BranchId" });
            DropIndex("dbo.PriceInvoices", new[] { "CustomerId" });
            DropIndex("dbo.PriceInvoicesDetails", new[] { "DeletedBy" });
            DropIndex("dbo.PriceInvoicesDetails", new[] { "ModifiedBy" });
            DropIndex("dbo.PriceInvoicesDetails", new[] { "CreatedBy" });
            DropIndex("dbo.PriceInvoicesDetails", new[] { "ItemId" });
            DropIndex("dbo.PriceInvoicesDetails", new[] { "PriceInvoiceId" });
            DropIndex("dbo.ItemTypes", new[] { "DeletedBy" });
            DropIndex("dbo.ItemTypes", new[] { "ModifiedBy" });
            DropIndex("dbo.ItemTypes", new[] { "CreatedBy" });
            DropIndex("dbo.ItemProductions", new[] { "DeletedBy" });
            DropIndex("dbo.ItemProductions", new[] { "ModifiedBy" });
            DropIndex("dbo.ItemProductions", new[] { "CreatedBy" });
            DropIndex("dbo.ItemProductions", new[] { "CustomerId" });
            DropIndex("dbo.ItemProductions", new[] { "ItemFinalId" });
            DropIndex("dbo.ItemProductionDetails", new[] { "DeletedBy" });
            DropIndex("dbo.ItemProductionDetails", new[] { "ModifiedBy" });
            DropIndex("dbo.ItemProductionDetails", new[] { "CreatedBy" });
            DropIndex("dbo.ItemProductionDetails", new[] { "ItemId" });
            DropIndex("dbo.ItemProductionDetails", new[] { "ItemProductionId" });
            DropIndex("dbo.GroupTypes", new[] { "DeletedBy" });
            DropIndex("dbo.GroupTypes", new[] { "ModifiedBy" });
            DropIndex("dbo.GroupTypes", new[] { "CreatedBy" });
            DropIndex("dbo.Groups", new[] { "DeletedBy" });
            DropIndex("dbo.Groups", new[] { "ModifiedBy" });
            DropIndex("dbo.Groups", new[] { "CreatedBy" });
            DropIndex("dbo.Groups", new[] { "ParentId" });
            DropIndex("dbo.Groups", new[] { "GroupTypeId" });
            DropIndex("dbo.StoreAdjustments", new[] { "DeletedBy" });
            DropIndex("dbo.StoreAdjustments", new[] { "ModifiedBy" });
            DropIndex("dbo.StoreAdjustments", new[] { "CreatedBy" });
            DropIndex("dbo.StoreAdjustments", new[] { "CustomerId" });
            DropIndex("dbo.StoreAdjustments", new[] { "ItemId" });
            DropIndex("dbo.StoreAdjustments", new[] { "StoreId" });
            DropIndex("dbo.SellInvoicesDetails", new[] { "DeletedBy" });
            DropIndex("dbo.SellInvoicesDetails", new[] { "ModifiedBy" });
            DropIndex("dbo.SellInvoicesDetails", new[] { "CreatedBy" });
            DropIndex("dbo.SellInvoicesDetails", new[] { "ItemSerialId" });
            DropIndex("dbo.SellInvoicesDetails", new[] { "ProductionOrderId" });
            DropIndex("dbo.SellInvoicesDetails", new[] { "ItemId" });
            DropIndex("dbo.SellInvoicesDetails", new[] { "StoreId" });
            DropIndex("dbo.SellInvoicesDetails", new[] { "SellInvoiceId" });
            DropIndex("dbo.SaleMenStoreHistories", new[] { "DeletedBy" });
            DropIndex("dbo.SaleMenStoreHistories", new[] { "ModifiedBy" });
            DropIndex("dbo.SaleMenStoreHistories", new[] { "CreatedBy" });
            DropIndex("dbo.SaleMenStoreHistories", new[] { "StoreId" });
            DropIndex("dbo.SaleMenStoreHistories", new[] { "EmployeeId" });
            DropIndex("dbo.SupplierPayments", new[] { "DeletedBy" });
            DropIndex("dbo.SupplierPayments", new[] { "ModifiedBy" });
            DropIndex("dbo.SupplierPayments", new[] { "CreatedBy" });
            DropIndex("dbo.SupplierPayments", new[] { "BankAccountId" });
            DropIndex("dbo.SupplierPayments", new[] { "SafeId" });
            DropIndex("dbo.SupplierPayments", new[] { "BranchId" });
            DropIndex("dbo.SupplierPayments", new[] { "PurchaseInvoiceId" });
            DropIndex("dbo.SupplierPayments", new[] { "SupplierId" });
            DropIndex("dbo.SellBackInvoicesDetails", new[] { "DeletedBy" });
            DropIndex("dbo.SellBackInvoicesDetails", new[] { "ModifiedBy" });
            DropIndex("dbo.SellBackInvoicesDetails", new[] { "CreatedBy" });
            DropIndex("dbo.SellBackInvoicesDetails", new[] { "ItemSerialId" });
            DropIndex("dbo.SellBackInvoicesDetails", new[] { "ItemId" });
            DropIndex("dbo.SellBackInvoicesDetails", new[] { "StoreId" });
            DropIndex("dbo.SellBackInvoicesDetails", new[] { "SellBackInvoiceId" });
            DropIndex("dbo.SellBackInvoiceIncomes", new[] { "DeletedBy" });
            DropIndex("dbo.SellBackInvoiceIncomes", new[] { "ModifiedBy" });
            DropIndex("dbo.SellBackInvoiceIncomes", new[] { "CreatedBy" });
            DropIndex("dbo.SellBackInvoiceIncomes", new[] { "IncomeTypeAccountTreeId" });
            DropIndex("dbo.SellBackInvoiceIncomes", new[] { "SellBackInvoiceId" });
            DropIndex("dbo.CasesSellInvoiceHistories", new[] { "DeletedBy" });
            DropIndex("dbo.CasesSellInvoiceHistories", new[] { "ModifiedBy" });
            DropIndex("dbo.CasesSellInvoiceHistories", new[] { "CreatedBy" });
            DropIndex("dbo.CasesSellInvoiceHistories", new[] { "SellBackInvoiceId" });
            DropIndex("dbo.CasesSellInvoiceHistories", new[] { "SellInvoiceId" });
            DropIndex("dbo.CasesSellInvoiceHistories", new[] { "CaseId" });
            DropIndex("dbo.SellBackInvoices", new[] { "DeletedBy" });
            DropIndex("dbo.SellBackInvoices", new[] { "ModifiedBy" });
            DropIndex("dbo.SellBackInvoices", new[] { "CreatedBy" });
            DropIndex("dbo.SellBackInvoices", new[] { "CaseId" });
            DropIndex("dbo.SellBackInvoices", new[] { "ShiftOfflineID" });
            DropIndex("dbo.SellBackInvoices", new[] { "BankAccountId" });
            DropIndex("dbo.SellBackInvoices", new[] { "SafeId" });
            DropIndex("dbo.SellBackInvoices", new[] { "PaymentTypeId" });
            DropIndex("dbo.SellBackInvoices", new[] { "BranchId" });
            DropIndex("dbo.SellBackInvoices", new[] { "CustomerId" });
            DropIndex("dbo.SellBackInvoices", new[] { "SellInvoiceId" });
            DropIndex("dbo.SellBackInvoices", new[] { "EmployeeId" });
            DropIndex("dbo.Offers", new[] { "DeletedBy" });
            DropIndex("dbo.Offers", new[] { "ModifiedBy" });
            DropIndex("dbo.Offers", new[] { "CreatedBy" });
            DropIndex("dbo.Offers", new[] { "UnitId" });
            DropIndex("dbo.ItemUnits", new[] { "DeletedBy" });
            DropIndex("dbo.ItemUnits", new[] { "ModifiedBy" });
            DropIndex("dbo.ItemUnits", new[] { "CreatedBy" });
            DropIndex("dbo.ItemUnits", new[] { "UnitId" });
            DropIndex("dbo.ItemUnits", new[] { "ItemId" });
            DropIndex("dbo.Units", new[] { "DeletedBy" });
            DropIndex("dbo.Units", new[] { "ModifiedBy" });
            DropIndex("dbo.Units", new[] { "CreatedBy" });
            DropIndex("dbo.ItemPrices", new[] { "DeletedBy" });
            DropIndex("dbo.ItemPrices", new[] { "ModifiedBy" });
            DropIndex("dbo.ItemPrices", new[] { "CreatedBy" });
            DropIndex("dbo.ItemPrices", new[] { "UnitId" });
            DropIndex("dbo.ItemPrices", new[] { "CustomerId" });
            DropIndex("dbo.ItemPrices", new[] { "PricingPolicyId" });
            DropIndex("dbo.ItemPrices", new[] { "ItemId" });
            DropIndex("dbo.PricingPolicies", new[] { "DeletedBy" });
            DropIndex("dbo.PricingPolicies", new[] { "ModifiedBy" });
            DropIndex("dbo.PricingPolicies", new[] { "CreatedBy" });
            DropIndex("dbo.PointOfSales", new[] { "DeletedBy" });
            DropIndex("dbo.PointOfSales", new[] { "ModifiedBy" });
            DropIndex("dbo.PointOfSales", new[] { "CreatedBy" });
            DropIndex("dbo.PointOfSales", new[] { "DefaultPaymentID" });
            DropIndex("dbo.PointOfSales", new[] { "DefaultSupplierID" });
            DropIndex("dbo.PointOfSales", new[] { "DefaultCustomerID" });
            DropIndex("dbo.PointOfSales", new[] { "DefaultPricePolicyID" });
            DropIndex("dbo.PointOfSales", new[] { "BankAccountID" });
            DropIndex("dbo.PointOfSales", new[] { "SafeID" });
            DropIndex("dbo.PointOfSales", new[] { "BrunchId" });
            DropIndex("dbo.ExpenseIncomes", new[] { "DeletedBy" });
            DropIndex("dbo.ExpenseIncomes", new[] { "ModifiedBy" });
            DropIndex("dbo.ExpenseIncomes", new[] { "CreatedBy" });
            DropIndex("dbo.ExpenseIncomes", new[] { "ShiftOffLineID" });
            DropIndex("dbo.ExpenseIncomes", new[] { "SafeId" });
            DropIndex("dbo.ExpenseIncomes", new[] { "BranchId" });
            DropIndex("dbo.ExpenseIncomes", new[] { "ExpenseIncomeTypeAccountTreeId" });
            DropIndex("dbo.ShiftsOfflines", new[] { "DeletedBy" });
            DropIndex("dbo.ShiftsOfflines", new[] { "ModifiedBy" });
            DropIndex("dbo.ShiftsOfflines", new[] { "CreatedBy" });
            DropIndex("dbo.ShiftsOfflines", new[] { "ClosedBy" });
            DropIndex("dbo.ShiftsOfflines", new[] { "EmployeeID" });
            DropIndex("dbo.ShiftsOfflines", new[] { "PointOfSaleID" });
            DropIndex("dbo.PurchaseInvoicesExpens", new[] { "DeletedBy" });
            DropIndex("dbo.PurchaseInvoicesExpens", new[] { "ModifiedBy" });
            DropIndex("dbo.PurchaseInvoicesExpens", new[] { "CreatedBy" });
            DropIndex("dbo.PurchaseInvoicesExpens", new[] { "ExpenseTypeAccountTreeId" });
            DropIndex("dbo.PurchaseInvoicesExpens", new[] { "PurchaseInvoiceId" });
            DropIndex("dbo.PurchaseInvoices", new[] { "DeletedBy" });
            DropIndex("dbo.PurchaseInvoices", new[] { "ModifiedBy" });
            DropIndex("dbo.PurchaseInvoices", new[] { "CreatedBy" });
            DropIndex("dbo.PurchaseInvoices", new[] { "ShiftOfflineID" });
            DropIndex("dbo.PurchaseInvoices", new[] { "CaseId" });
            DropIndex("dbo.PurchaseInvoices", new[] { "BankAccountId" });
            DropIndex("dbo.PurchaseInvoices", new[] { "SafeId" });
            DropIndex("dbo.PurchaseInvoices", new[] { "PaymentTypeId" });
            DropIndex("dbo.PurchaseInvoices", new[] { "BranchId" });
            DropIndex("dbo.PurchaseInvoices", new[] { "SupplierId" });
            DropIndex("dbo.PurchaseInvoicesDetails", new[] { "DeletedBy" });
            DropIndex("dbo.PurchaseInvoicesDetails", new[] { "ModifiedBy" });
            DropIndex("dbo.PurchaseInvoicesDetails", new[] { "CreatedBy" });
            DropIndex("dbo.PurchaseInvoicesDetails", new[] { "StoreId" });
            DropIndex("dbo.PurchaseInvoicesDetails", new[] { "PurchaseInvoiceId" });
            DropIndex("dbo.PurchaseInvoicesDetails", new[] { "ItemId" });
            DropIndex("dbo.PurchaseInvoicesDetails", new[] { "ContainerId" });
            DropIndex("dbo.Containers", new[] { "DeletedBy" });
            DropIndex("dbo.Containers", new[] { "ModifiedBy" });
            DropIndex("dbo.Containers", new[] { "CreatedBy" });
            DropIndex("dbo.PurchaseBackInvoicesDetails", new[] { "DeletedBy" });
            DropIndex("dbo.PurchaseBackInvoicesDetails", new[] { "ModifiedBy" });
            DropIndex("dbo.PurchaseBackInvoicesDetails", new[] { "CreatedBy" });
            DropIndex("dbo.PurchaseBackInvoicesDetails", new[] { "StoreId" });
            DropIndex("dbo.PurchaseBackInvoicesDetails", new[] { "PurchaseBackInvoiceId" });
            DropIndex("dbo.PurchaseBackInvoicesDetails", new[] { "ItemId" });
            DropIndex("dbo.PurchaseBackInvoicesDetails", new[] { "ContainerId" });
            DropIndex("dbo.MaintenProblemTypes", new[] { "DeletedBy" });
            DropIndex("dbo.MaintenProblemTypes", new[] { "ModifiedBy" });
            DropIndex("dbo.MaintenProblemTypes", new[] { "CreatedBy" });
            DropIndex("dbo.MaintenanceSpareParts", new[] { "DeletedBy" });
            DropIndex("dbo.MaintenanceSpareParts", new[] { "ModifiedBy" });
            DropIndex("dbo.MaintenanceSpareParts", new[] { "CreatedBy" });
            DropIndex("dbo.MaintenanceSpareParts", new[] { "ProductionOrderId" });
            DropIndex("dbo.MaintenanceSpareParts", new[] { "ItemSerialId" });
            DropIndex("dbo.MaintenanceSpareParts", new[] { "ItemId" });
            DropIndex("dbo.MaintenanceSpareParts", new[] { "StoreId" });
            DropIndex("dbo.MaintenanceSpareParts", new[] { "MaintenanceDetailId" });
            DropIndex("dbo.MaintenanceIncomes", new[] { "DeletedBy" });
            DropIndex("dbo.MaintenanceIncomes", new[] { "ModifiedBy" });
            DropIndex("dbo.MaintenanceIncomes", new[] { "CreatedBy" });
            DropIndex("dbo.MaintenanceIncomes", new[] { "MaintenanceDetailId" });
            DropIndex("dbo.MaintenanceCaseHistories", new[] { "DeletedBy" });
            DropIndex("dbo.MaintenanceCaseHistories", new[] { "ModifiedBy" });
            DropIndex("dbo.MaintenanceCaseHistories", new[] { "CreatedBy" });
            DropIndex("dbo.MaintenanceCaseHistories", new[] { "MaintenanceId" });
            DropIndex("dbo.MaintenanceCaseHistories", new[] { "MaintenanceCaseId" });
            DropIndex("dbo.MaintenanceCas", new[] { "DeletedBy" });
            DropIndex("dbo.MaintenanceCas", new[] { "ModifiedBy" });
            DropIndex("dbo.MaintenanceCas", new[] { "CreatedBy" });
            DropIndex("dbo.MaintenanceDetails", new[] { "DeletedBy" });
            DropIndex("dbo.MaintenanceDetails", new[] { "ModifiedBy" });
            DropIndex("dbo.MaintenanceDetails", new[] { "CreatedBy" });
            DropIndex("dbo.MaintenanceDetails", new[] { "MaintenanceCaseId" });
            DropIndex("dbo.MaintenanceDetails", new[] { "MaintenProblemTypeId" });
            DropIndex("dbo.MaintenanceDetails", new[] { "ItemSerialId" });
            DropIndex("dbo.MaintenanceDetails", new[] { "ItemId" });
            DropIndex("dbo.MaintenanceDetails", new[] { "MaintenanceId" });
            DropIndex("dbo.MaintenanceDamageDetails", new[] { "DeletedBy" });
            DropIndex("dbo.MaintenanceDamageDetails", new[] { "ModifiedBy" });
            DropIndex("dbo.MaintenanceDamageDetails", new[] { "CreatedBy" });
            DropIndex("dbo.MaintenanceDamageDetails", new[] { "ItemId" });
            DropIndex("dbo.MaintenanceDamageDetails", new[] { "MaintenanceDamageId" });
            DropIndex("dbo.MaintenanceDamages", new[] { "DeletedBy" });
            DropIndex("dbo.MaintenanceDamages", new[] { "ModifiedBy" });
            DropIndex("dbo.MaintenanceDamages", new[] { "CreatedBy" });
            DropIndex("dbo.MaintenanceDamages", new[] { "StoreId" });
            DropIndex("dbo.MaintenanceDamages", new[] { "MaintenanceDetailId" });
            DropIndex("dbo.ItemIntialBalances", new[] { "DeletedBy" });
            DropIndex("dbo.ItemIntialBalances", new[] { "ModifiedBy" });
            DropIndex("dbo.ItemIntialBalances", new[] { "CreatedBy" });
            DropIndex("dbo.ItemIntialBalances", new[] { "StoreId" });
            DropIndex("dbo.ItemIntialBalances", new[] { "ItemId" });
            DropIndex("dbo.ItemIntialBalances", new[] { "AccountTreeId" });
            DropIndex("dbo.StoresTransfers", new[] { "DeletedBy" });
            DropIndex("dbo.StoresTransfers", new[] { "ModifiedBy" });
            DropIndex("dbo.StoresTransfers", new[] { "CreatedBy" });
            DropIndex("dbo.StoresTransfers", new[] { "EmployeeToId" });
            DropIndex("dbo.StoresTransfers", new[] { "EmployeeFromId" });
            DropIndex("dbo.StoresTransfers", new[] { "StoreToId" });
            DropIndex("dbo.StoresTransfers", new[] { "StoreFromId" });
            DropIndex("dbo.StoresTransferDetails", new[] { "DeletedBy" });
            DropIndex("dbo.StoresTransferDetails", new[] { "ModifiedBy" });
            DropIndex("dbo.StoresTransferDetails", new[] { "CreatedBy" });
            DropIndex("dbo.StoresTransferDetails", new[] { "ItemId" });
            DropIndex("dbo.StoresTransferDetails", new[] { "ItemSerialId" });
            DropIndex("dbo.StoresTransferDetails", new[] { "ProductionOrderId" });
            DropIndex("dbo.StoresTransferDetails", new[] { "StoresTransferId" });
            DropIndex("dbo.ProductionOrderReceipts", new[] { "DeletedBy" });
            DropIndex("dbo.ProductionOrderReceipts", new[] { "ModifiedBy" });
            DropIndex("dbo.ProductionOrderReceipts", new[] { "CreatedBy" });
            DropIndex("dbo.ProductionOrderReceipts", new[] { "FinalItemStoreId" });
            DropIndex("dbo.ProductionOrderReceipts", new[] { "ProductionOrderId" });
            DropIndex("dbo.ProductionOrderExpens", new[] { "DeletedBy" });
            DropIndex("dbo.ProductionOrderExpens", new[] { "ModifiedBy" });
            DropIndex("dbo.ProductionOrderExpens", new[] { "CreatedBy" });
            DropIndex("dbo.ProductionOrderExpens", new[] { "ExpenseTypeAccountTreeId" });
            DropIndex("dbo.ProductionOrderExpens", new[] { "ProductionOrderId" });
            DropIndex("dbo.ProductionOrderColors", new[] { "DeletedBy" });
            DropIndex("dbo.ProductionOrderColors", new[] { "ModifiedBy" });
            DropIndex("dbo.ProductionOrderColors", new[] { "CreatedBy" });
            DropIndex("dbo.ProductionOrders", new[] { "DeletedBy" });
            DropIndex("dbo.ProductionOrders", new[] { "ModifiedBy" });
            DropIndex("dbo.ProductionOrders", new[] { "CreatedBy" });
            DropIndex("dbo.ProductionOrders", new[] { "ProductionStoreId" });
            DropIndex("dbo.ProductionOrders", new[] { "FinalItemId" });
            DropIndex("dbo.ProductionOrders", new[] { "BranchId" });
            DropIndex("dbo.ProductionOrders", new[] { "OrderColorId" });
            DropIndex("dbo.ProductionOrderDetails", new[] { "DeletedBy" });
            DropIndex("dbo.ProductionOrderDetails", new[] { "ModifiedBy" });
            DropIndex("dbo.ProductionOrderDetails", new[] { "CreatedBy" });
            DropIndex("dbo.ProductionOrderDetails", new[] { "ItemCostCalculateId" });
            DropIndex("dbo.ProductionOrderDetails", new[] { "StoreId" });
            DropIndex("dbo.ProductionOrderDetails", new[] { "ItemId" });
            DropIndex("dbo.ProductionOrderDetails", new[] { "ProductionOrderId" });
            DropIndex("dbo.ItemCostCalculations", new[] { "DeletedBy" });
            DropIndex("dbo.ItemCostCalculations", new[] { "ModifiedBy" });
            DropIndex("dbo.ItemCostCalculations", new[] { "CreatedBy" });
            DropIndex("dbo.InventoryInvoiceDetails", new[] { "DeletedBy" });
            DropIndex("dbo.InventoryInvoiceDetails", new[] { "ModifiedBy" });
            DropIndex("dbo.InventoryInvoiceDetails", new[] { "CreatedBy" });
            DropIndex("dbo.InventoryInvoiceDetails", new[] { "ItemId" });
            DropIndex("dbo.InventoryInvoiceDetails", new[] { "InventoryInvoiceId" });
            DropIndex("dbo.InventoryInvoices", new[] { "DeletedBy" });
            DropIndex("dbo.InventoryInvoices", new[] { "ModifiedBy" });
            DropIndex("dbo.InventoryInvoices", new[] { "CreatedBy" });
            DropIndex("dbo.InventoryInvoices", new[] { "ItemCostCalculateId" });
            DropIndex("dbo.InventoryInvoices", new[] { "StoreId" });
            DropIndex("dbo.InventoryInvoices", new[] { "BranchId" });
            DropIndex("dbo.Stores", new[] { "Employee_Id" });
            DropIndex("dbo.Stores", new[] { "DeletedBy" });
            DropIndex("dbo.Stores", new[] { "ModifiedBy" });
            DropIndex("dbo.Stores", new[] { "CreatedBy" });
            DropIndex("dbo.Stores", new[] { "EmployeeId" });
            DropIndex("dbo.Stores", new[] { "BranchId" });
            DropIndex("dbo.Stores", new[] { "AccountTreeId" });
            DropIndex("dbo.BalanceFirstDurations", new[] { "DeletedBy" });
            DropIndex("dbo.BalanceFirstDurations", new[] { "ModifiedBy" });
            DropIndex("dbo.BalanceFirstDurations", new[] { "CreatedBy" });
            DropIndex("dbo.BalanceFirstDurations", new[] { "ItemId" });
            DropIndex("dbo.BalanceFirstDurations", new[] { "StoreId" });
            DropIndex("dbo.Items", new[] { "DeletedBy" });
            DropIndex("dbo.Items", new[] { "ModifiedBy" });
            DropIndex("dbo.Items", new[] { "CreatedBy" });
            DropIndex("dbo.Items", new[] { "UnitConvertFromId" });
            DropIndex("dbo.Items", new[] { "UnitId" });
            DropIndex("dbo.Items", new[] { "ItemTypeId" });
            DropIndex("dbo.Items", new[] { "GroupSellId" });
            DropIndex("dbo.Items", new[] { "GroupBasicId" });
            DropIndex("dbo.SerialCas", new[] { "DeletedBy" });
            DropIndex("dbo.SerialCas", new[] { "ModifiedBy" });
            DropIndex("dbo.SerialCas", new[] { "CreatedBy" });
            DropIndex("dbo.CasesItemSerialHistories", new[] { "DeletedBy" });
            DropIndex("dbo.CasesItemSerialHistories", new[] { "ModifiedBy" });
            DropIndex("dbo.CasesItemSerialHistories", new[] { "CreatedBy" });
            DropIndex("dbo.CasesItemSerialHistories", new[] { "ItemSerialId" });
            DropIndex("dbo.CasesItemSerialHistories", new[] { "SerialCaseId" });
            DropIndex("dbo.ItemSerials", new[] { "DeletedBy" });
            DropIndex("dbo.ItemSerials", new[] { "ModifiedBy" });
            DropIndex("dbo.ItemSerials", new[] { "CreatedBy" });
            DropIndex("dbo.ItemSerials", new[] { "CurrentStoreId" });
            DropIndex("dbo.ItemSerials", new[] { "SellInvoiceId" });
            DropIndex("dbo.ItemSerials", new[] { "MaintenanceId" });
            DropIndex("dbo.ItemSerials", new[] { "ProductionOrderId" });
            DropIndex("dbo.ItemSerials", new[] { "SerialCaseId" });
            DropIndex("dbo.ItemSerials", new[] { "ItemId" });
            DropIndex("dbo.Maintenances", new[] { "DeletedBy" });
            DropIndex("dbo.Maintenances", new[] { "ModifiedBy" });
            DropIndex("dbo.Maintenances", new[] { "CreatedBy" });
            DropIndex("dbo.Maintenances", new[] { "StoreReceiptId" });
            DropIndex("dbo.Maintenances", new[] { "MaintenanceCaseId" });
            DropIndex("dbo.Maintenances", new[] { "BankAccountId" });
            DropIndex("dbo.Maintenances", new[] { "SafeId" });
            DropIndex("dbo.Maintenances", new[] { "PaymentTypeId" });
            DropIndex("dbo.Maintenances", new[] { "BranchId" });
            DropIndex("dbo.Maintenances", new[] { "CustomerId" });
            DropIndex("dbo.Maintenances", new[] { "SellInvoiceId" });
            DropIndex("dbo.Maintenances", new[] { "EmployeeResponseId" });
            DropIndex("dbo.Maintenances", new[] { "EmployeeSaleMenId" });
            DropIndex("dbo.Maintenances", new[] { "StoreId" });
            DropIndex("dbo.PaymentTypes", new[] { "DeletedBy" });
            DropIndex("dbo.PaymentTypes", new[] { "ModifiedBy" });
            DropIndex("dbo.PaymentTypes", new[] { "CreatedBy" });
            DropIndex("dbo.PurchaseBackInvoices", new[] { "DeletedBy" });
            DropIndex("dbo.PurchaseBackInvoices", new[] { "ModifiedBy" });
            DropIndex("dbo.PurchaseBackInvoices", new[] { "CreatedBy" });
            DropIndex("dbo.PurchaseBackInvoices", new[] { "ShiftOfflineID" });
            DropIndex("dbo.PurchaseBackInvoices", new[] { "CaseId" });
            DropIndex("dbo.PurchaseBackInvoices", new[] { "BankAccountId" });
            DropIndex("dbo.PurchaseBackInvoices", new[] { "SafeId" });
            DropIndex("dbo.PurchaseBackInvoices", new[] { "PaymentTypeId" });
            DropIndex("dbo.PurchaseBackInvoices", new[] { "BranchId" });
            DropIndex("dbo.PurchaseBackInvoices", new[] { "PurchaseInvoiceId" });
            DropIndex("dbo.PurchaseBackInvoices", new[] { "SupplierId" });
            DropIndex("dbo.CasesPurchaseInvoiceHistories", new[] { "DeletedBy" });
            DropIndex("dbo.CasesPurchaseInvoiceHistories", new[] { "ModifiedBy" });
            DropIndex("dbo.CasesPurchaseInvoiceHistories", new[] { "CreatedBy" });
            DropIndex("dbo.CasesPurchaseInvoiceHistories", new[] { "PurchaseBackInvoiceId" });
            DropIndex("dbo.CasesPurchaseInvoiceHistories", new[] { "PurchaseInvoiceId" });
            DropIndex("dbo.CasesPurchaseInvoiceHistories", new[] { "CaseId" });
            DropIndex("dbo.Cases", new[] { "DeletedBy" });
            DropIndex("dbo.Cases", new[] { "ModifiedBy" });
            DropIndex("dbo.Cases", new[] { "CreatedBy" });
            DropIndex("dbo.SellInvoices", new[] { "DeletedBy" });
            DropIndex("dbo.SellInvoices", new[] { "ModifiedBy" });
            DropIndex("dbo.SellInvoices", new[] { "CreatedBy" });
            DropIndex("dbo.SellInvoices", new[] { "CaseId" });
            DropIndex("dbo.SellInvoices", new[] { "ShiftOffLineID" });
            DropIndex("dbo.SellInvoices", new[] { "BankAccountId" });
            DropIndex("dbo.SellInvoices", new[] { "SafeId" });
            DropIndex("dbo.SellInvoices", new[] { "PaymentTypeId" });
            DropIndex("dbo.SellInvoices", new[] { "BranchId" });
            DropIndex("dbo.SellInvoices", new[] { "CustomerId" });
            DropIndex("dbo.SellInvoices", new[] { "EmployeeId" });
            DropIndex("dbo.CustomerPayments", new[] { "DeletedBy" });
            DropIndex("dbo.CustomerPayments", new[] { "ModifiedBy" });
            DropIndex("dbo.CustomerPayments", new[] { "CreatedBy" });
            DropIndex("dbo.CustomerPayments", new[] { "SafeId" });
            DropIndex("dbo.CustomerPayments", new[] { "BranchId" });
            DropIndex("dbo.CustomerPayments", new[] { "SellInvoiceId" });
            DropIndex("dbo.CustomerPayments", new[] { "EmployeeId" });
            DropIndex("dbo.CustomerPayments", new[] { "CustomerId" });
            DropIndex("dbo.Safes", new[] { "DeletedBy" });
            DropIndex("dbo.Safes", new[] { "ModifiedBy" });
            DropIndex("dbo.Safes", new[] { "CreatedBy" });
            DropIndex("dbo.Safes", new[] { "BranchId" });
            DropIndex("dbo.Safes", new[] { "ResoponsiblePersonId" });
            DropIndex("dbo.Safes", new[] { "AccountsTreeId" });
            DropIndex("dbo.ContractLoanSchedulings", new[] { "DeletedBy" });
            DropIndex("dbo.ContractLoanSchedulings", new[] { "ModifiedBy" });
            DropIndex("dbo.ContractLoanSchedulings", new[] { "CreatedBy" });
            DropIndex("dbo.ContractLoanSchedulings", new[] { "ContractSchedulingId" });
            DropIndex("dbo.ContractLoanSchedulings", new[] { "ContractLoanId" });
            DropIndex("dbo.ContractLoans", new[] { "DeletedBy" });
            DropIndex("dbo.ContractLoans", new[] { "ModifiedBy" });
            DropIndex("dbo.ContractLoans", new[] { "CreatedBy" });
            DropIndex("dbo.ContractLoans", new[] { "BankAccountId" });
            DropIndex("dbo.ContractLoans", new[] { "SafeId" });
            DropIndex("dbo.ContractLoans", new[] { "BranchId" });
            DropIndex("dbo.ContractLoans", new[] { "ContractSchedulingId" });
            DropIndex("dbo.ContractAttendanceLeavings", new[] { "DeletedBy" });
            DropIndex("dbo.ContractAttendanceLeavings", new[] { "ModifiedBy" });
            DropIndex("dbo.ContractAttendanceLeavings", new[] { "CreatedBy" });
            DropIndex("dbo.ContractAttendanceLeavings", new[] { "ContractSchedulingId" });
            DropIndex("dbo.ContractSchedulings", new[] { "DeletedBy" });
            DropIndex("dbo.ContractSchedulings", new[] { "ModifiedBy" });
            DropIndex("dbo.ContractSchedulings", new[] { "CreatedBy" });
            DropIndex("dbo.ContractSchedulings", new[] { "ContractId" });
            DropIndex("dbo.ContractSchedulingAbsences", new[] { "DeletedBy" });
            DropIndex("dbo.ContractSchedulingAbsences", new[] { "ModifiedBy" });
            DropIndex("dbo.ContractSchedulingAbsences", new[] { "CreatedBy" });
            DropIndex("dbo.ContractSchedulingAbsences", new[] { "VacationTypeId" });
            DropIndex("dbo.ContractSchedulingAbsences", new[] { "ContractSchedulingId" });
            DropIndex("dbo.VacationTypes", new[] { "DeletedBy" });
            DropIndex("dbo.VacationTypes", new[] { "ModifiedBy" });
            DropIndex("dbo.VacationTypes", new[] { "CreatedBy" });
            DropIndex("dbo.ContractVacations", new[] { "DeletedBy" });
            DropIndex("dbo.ContractVacations", new[] { "ModifiedBy" });
            DropIndex("dbo.ContractVacations", new[] { "CreatedBy" });
            DropIndex("dbo.ContractVacations", new[] { "ApprovalBy" });
            DropIndex("dbo.ContractVacations", new[] { "VacationTypeId" });
            DropIndex("dbo.ContractVacations", new[] { "ContractId" });
            DropIndex("dbo.People", new[] { "DeletedBy" });
            DropIndex("dbo.People", new[] { "ModifiedBy" });
            DropIndex("dbo.People", new[] { "CreatedBy" });
            DropIndex("dbo.People", new[] { "GenderId" });
            DropIndex("dbo.People", new[] { "ParentId" });
            DropIndex("dbo.People", new[] { "AreaId" });
            DropIndex("dbo.People", new[] { "PersonCategoryId" });
            DropIndex("dbo.People", new[] { "PersonTypeId" });
            DropIndex("dbo.People", new[] { "AccountTreeCustomerEmpId" });
            DropIndex("dbo.People", new[] { "AccountTreeSupplierId" });
            DropIndex("dbo.People", new[] { "AccountsTreeCustomerId" });
            DropIndex("dbo.Users", new[] { "Role_Id" });
            DropIndex("dbo.Users", new[] { "Person_Id" });
            DropIndex("dbo.Users", new[] { "DeletedBy" });
            DropIndex("dbo.Users", new[] { "ModifiedBy" });
            DropIndex("dbo.Users", new[] { "CreatedBy" });
            DropIndex("dbo.Users", new[] { "RoleId" });
            DropIndex("dbo.Users", new[] { "PersonId" });
            DropIndex("dbo.ContractDefinitionVacations", new[] { "DeletedBy" });
            DropIndex("dbo.ContractDefinitionVacations", new[] { "ModifiedBy" });
            DropIndex("dbo.ContractDefinitionVacations", new[] { "CreatedBy" });
            DropIndex("dbo.ContractDefinitionVacations", new[] { "VacationTypeId" });
            DropIndex("dbo.ContractDefinitionVacations", new[] { "ContractId" });
            DropIndex("dbo.Contracts", new[] { "DeletedBy" });
            DropIndex("dbo.Contracts", new[] { "ModifiedBy" });
            DropIndex("dbo.Contracts", new[] { "CreatedBy" });
            DropIndex("dbo.Contracts", new[] { "EmployeeId" });
            DropIndex("dbo.Contracts", new[] { "ContractSalaryTypeId" });
            DropIndex("dbo.Contracts", new[] { "ContractTypeId" });
            DropIndex("dbo.Employees", new[] { "Store_Id" });
            DropIndex("dbo.Employees", new[] { "DeletedBy" });
            DropIndex("dbo.Employees", new[] { "ModifiedBy" });
            DropIndex("dbo.Employees", new[] { "CreatedBy" });
            DropIndex("dbo.Employees", new[] { "SocialStatusId" });
            DropIndex("dbo.Employees", new[] { "JobId" });
            DropIndex("dbo.Employees", new[] { "DepartmentId" });
            DropIndex("dbo.Employees", new[] { "StoreId" });
            DropIndex("dbo.Employees", new[] { "BranchId" });
            DropIndex("dbo.Employees", new[] { "PersonId" });
            DropIndex("dbo.Cheques", new[] { "DeletedBy" });
            DropIndex("dbo.Cheques", new[] { "ModifiedBy" });
            DropIndex("dbo.Cheques", new[] { "CreatedBy" });
            DropIndex("dbo.Cheques", new[] { "BankAccountId" });
            DropIndex("dbo.Cheques", new[] { "CustomerId" });
            DropIndex("dbo.Cheques", new[] { "SupplierId" });
            DropIndex("dbo.Cheques", new[] { "BranchId" });
            DropIndex("dbo.Cheques", new[] { "EmployeeId" });
            DropIndex("dbo.Branches", new[] { "DeletedBy" });
            DropIndex("dbo.Branches", new[] { "ModifiedBy" });
            DropIndex("dbo.Branches", new[] { "CreatedBy" });
            DropIndex("dbo.Branches", new[] { "AreaId" });
            DropIndex("dbo.Areas", new[] { "DeletedBy" });
            DropIndex("dbo.Areas", new[] { "ModifiedBy" });
            DropIndex("dbo.Areas", new[] { "CreatedBy" });
            DropIndex("dbo.Areas", new[] { "CityId" });
            DropIndex("dbo.Cities", new[] { "DeletedBy" });
            DropIndex("dbo.Cities", new[] { "ModifiedBy" });
            DropIndex("dbo.Cities", new[] { "CreatedBy" });
            DropIndex("dbo.Cities", new[] { "CountryId" });
            DropIndex("dbo.Banks", new[] { "DeletedBy" });
            DropIndex("dbo.Banks", new[] { "ModifiedBy" });
            DropIndex("dbo.Banks", new[] { "CreatedBy" });
            DropIndex("dbo.Banks", new[] { "CityId" });
            DropIndex("dbo.BankAccounts", new[] { "DeletedBy" });
            DropIndex("dbo.BankAccounts", new[] { "ModifiedBy" });
            DropIndex("dbo.BankAccounts", new[] { "CreatedBy" });
            DropIndex("dbo.BankAccounts", new[] { "BankId" });
            DropIndex("dbo.BankAccounts", new[] { "AccountsTreeId" });
            DropIndex("dbo.Assets", new[] { "DeletedBy" });
            DropIndex("dbo.Assets", new[] { "ModifiedBy" });
            DropIndex("dbo.Assets", new[] { "CreatedBy" });
            DropIndex("dbo.Assets", new[] { "BankAccountId" });
            DropIndex("dbo.Assets", new[] { "SafeId" });
            DropIndex("dbo.Assets", new[] { "BranchId" });
            DropIndex("dbo.Assets", new[] { "AccountTreeDestructionId" });
            DropIndex("dbo.Assets", new[] { "AccountTreeExpenseId" });
            DropIndex("dbo.Assets", new[] { "AccountTreeParentId" });
            DropIndex("dbo.Assets", new[] { "AccountTreeId" });
            DropIndex("dbo.AccountsTrees", new[] { "DeletedBy" });
            DropIndex("dbo.AccountsTrees", new[] { "ModifiedBy" });
            DropIndex("dbo.AccountsTrees", new[] { "CreatedBy" });
            DropIndex("dbo.AccountsTrees", new[] { "TypeId" });
            DropIndex("dbo.AccountsTrees", new[] { "ParentId" });
            DropTable("dbo.UploadCenterTypes");
            DropTable("dbo.UploadCenters");
            DropTable("dbo.NotificationTypes");
            DropTable("dbo.Notifications");
            DropTable("dbo.GeneralSettingTypes");
            DropTable("dbo.GeneralSettings");
            DropTable("dbo.DelayAbsenceSystems");
            DropTable("dbo.ContractSchedulingsMains");
            DropTable("dbo.SelectorTypes");
            DropTable("dbo.IncomeTypes");
            DropTable("dbo.GeneralRecords");
            DropTable("dbo.ExpenseTypes");
            DropTable("dbo.Countries");
            DropTable("dbo.Vouchers");
            DropTable("dbo.TransactionsTypes");
            DropTable("dbo.GeneralDailies");
            DropTable("dbo.SocialStatus");
            DropTable("dbo.Jobs");
            DropTable("dbo.EmployeeReturnCustodies");
            DropTable("dbo.WeekDays");
            DropTable("dbo.VacationDays");
            DropTable("dbo.WorkingPeriods");
            DropTable("dbo.Shifts");
            DropTable("dbo.Departments");
            DropTable("dbo.ContractTypes");
            DropTable("dbo.ContractSalaryTypes");
            DropTable("dbo.Pages");
            DropTable("dbo.PagesRoles");
            DropTable("dbo.Roles");
            DropTable("dbo.SaleMenCustomers");
            DropTable("dbo.SaleMenAreas");
            DropTable("dbo.PersonTypes");
            DropTable("dbo.PersonCategories");
            DropTable("dbo.Genders");
            DropTable("dbo.SalaryPenaltyTypes");
            DropTable("dbo.SalaryPenalties");
            DropTable("dbo.ContractSalaryPenalties");
            DropTable("dbo.SalaryAdditionTypes");
            DropTable("dbo.SalaryAdditions");
            DropTable("dbo.ContractSalaryAdditions");
            DropTable("dbo.EmployeeReturnCashCustodies");
            DropTable("dbo.EmployeeGivingCustodies");
            DropTable("dbo.SellInvoicePayments");
            DropTable("dbo.SellInvoiceIncomes");
            DropTable("dbo.InstallmentSchedules");
            DropTable("dbo.Installments");
            DropTable("dbo.PurchaseBackInvoicesExpens");
            DropTable("dbo.PricesChanges");
            DropTable("dbo.PriceInvoices");
            DropTable("dbo.PriceInvoicesDetails");
            DropTable("dbo.ItemTypes");
            DropTable("dbo.ItemProductions");
            DropTable("dbo.ItemProductionDetails");
            DropTable("dbo.GroupTypes");
            DropTable("dbo.Groups");
            DropTable("dbo.StoreAdjustments");
            DropTable("dbo.SellInvoicesDetails");
            DropTable("dbo.SaleMenStoreHistories");
            DropTable("dbo.SupplierPayments");
            DropTable("dbo.SellBackInvoicesDetails");
            DropTable("dbo.SellBackInvoiceIncomes");
            DropTable("dbo.CasesSellInvoiceHistories");
            DropTable("dbo.SellBackInvoices");
            DropTable("dbo.Offers");
            DropTable("dbo.ItemUnits");
            DropTable("dbo.Units");
            DropTable("dbo.ItemPrices");
            DropTable("dbo.PricingPolicies");
            DropTable("dbo.PointOfSales");
            DropTable("dbo.ExpenseIncomes");
            DropTable("dbo.ShiftsOfflines");
            DropTable("dbo.PurchaseInvoicesExpens");
            DropTable("dbo.PurchaseInvoices");
            DropTable("dbo.PurchaseInvoicesDetails");
            DropTable("dbo.Containers");
            DropTable("dbo.PurchaseBackInvoicesDetails");
            DropTable("dbo.MaintenProblemTypes");
            DropTable("dbo.MaintenanceSpareParts");
            DropTable("dbo.MaintenanceIncomes");
            DropTable("dbo.MaintenanceCaseHistories");
            DropTable("dbo.MaintenanceCas");
            DropTable("dbo.MaintenanceDetails");
            DropTable("dbo.MaintenanceDamageDetails");
            DropTable("dbo.MaintenanceDamages");
            DropTable("dbo.ItemIntialBalances");
            DropTable("dbo.StoresTransfers");
            DropTable("dbo.StoresTransferDetails");
            DropTable("dbo.ProductionOrderReceipts");
            DropTable("dbo.ProductionOrderExpens");
            DropTable("dbo.ProductionOrderColors");
            DropTable("dbo.ProductionOrders");
            DropTable("dbo.ProductionOrderDetails");
            DropTable("dbo.ItemCostCalculations");
            DropTable("dbo.InventoryInvoiceDetails");
            DropTable("dbo.InventoryInvoices");
            DropTable("dbo.Stores");
            DropTable("dbo.BalanceFirstDurations");
            DropTable("dbo.Items");
            DropTable("dbo.SerialCas");
            DropTable("dbo.CasesItemSerialHistories");
            DropTable("dbo.ItemSerials");
            DropTable("dbo.Maintenances");
            DropTable("dbo.PaymentTypes");
            DropTable("dbo.PurchaseBackInvoices");
            DropTable("dbo.CasesPurchaseInvoiceHistories");
            DropTable("dbo.Cases");
            DropTable("dbo.SellInvoices");
            DropTable("dbo.CustomerPayments");
            DropTable("dbo.Safes");
            DropTable("dbo.ContractLoanSchedulings");
            DropTable("dbo.ContractLoans");
            DropTable("dbo.ContractAttendanceLeavings");
            DropTable("dbo.ContractSchedulings");
            DropTable("dbo.ContractSchedulingAbsences");
            DropTable("dbo.VacationTypes");
            DropTable("dbo.ContractVacations");
            DropTable("dbo.People");
            DropTable("dbo.Users");
            DropTable("dbo.ContractDefinitionVacations");
            DropTable("dbo.Contracts");
            DropTable("dbo.Employees");
            DropTable("dbo.Cheques");
            DropTable("dbo.Branches");
            DropTable("dbo.Areas");
            DropTable("dbo.Cities");
            DropTable("dbo.Banks");
            DropTable("dbo.BankAccounts");
            DropTable("dbo.Assets");
            DropTable("dbo.AccountsTrees");
        }
    }
}
