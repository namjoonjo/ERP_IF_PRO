// ERP_IF_PRO.Modules 네임스페이스의 클래스를 COMBINATION.Modules에서 접근 가능하도록 브릿지
// Regi_Combi_PR_Label.cs가 using COMBINATION.Modules만 사용하기 때문에 필요
namespace COMBINATION.Modules
{
    class MSSQL : ERP_IF_PRO.Modules.MSSQL
    {
        public MSSQL(string dbName) : base(dbName) { }
    }

    class CommonModule : ERP_IF_PRO.Modules.CommonModule { }
}
