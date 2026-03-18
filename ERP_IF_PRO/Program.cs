using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;

namespace ERP_IF_PRO
{
    internal static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 외부 DLL이 참조하는 어셈블리를 메인 앱의 bin 폴더에서 찾도록 처리
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            // DevExpress 스킨 설정
            UserLookAndFeel.Default.SetSkinStyle(SkinStyle.Office2019Colorful);

            // 로그인 화면
            LoginForm loginForm = new LoginForm();
            if (loginForm.ShowDialog() != DialogResult.OK)
            {
                return; // 취소 시 프로그램 종료
            }

            // Main 폼에 관리자 여부 전달
            Main mainForm = new Main();
            mainForm.IsAdmin = loginForm.IsAdmin;
            Application.Run(mainForm);
        }

        /// <summary>
        /// 외부 DLL이 참조하는 종속 어셈블리 해결
        /// (DevExpress, System 등 메인 앱에 이미 있는 어셈블리)
        /// </summary>
        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                // 이미 로드된 어셈블리에서 검색
                foreach (Assembly loaded in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (loaded.FullName == args.Name)
                        return loaded;
                }

                // 메인 앱의 bin 폴더에서 검색
                string assemblyName = new AssemblyName(args.Name).Name + ".dll";
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyName);

                if (File.Exists(path))
                    return Assembly.LoadFrom(path);
            }
            catch { }

            return null;
        }
    }
}
