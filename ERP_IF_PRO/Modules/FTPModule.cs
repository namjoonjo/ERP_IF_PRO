using System;
using System.Configuration;
using System.IO;
using System.Net;

namespace ERP_IF_PRO.Modules
{
    /***********************************
     *
     * FTP 모듈 (IIS FTP 서버 연동)
     * - FTP 서버에서 DLL 파일을 다운로드하는 모듈
     * - App.config의 appSettings에서 접속정보를 읽어옴
     * - 별도 NuGet 패키지 불필요 (System.Net.FtpWebRequest 사용)
     *
     * 사용방법:
     * 1. FTPModule ftp = new FTPModule();
     * 2. string localPath = ftp.DownloadDll("FormName.dll");
     * 3. localPath가 null이 아니면 다운로드 성공
     *
     ***********************************/

    class FTPModule
    {
        CommonModule cm = new CommonModule();

        private string host;
        private int port;
        private string username;
        private string password;
        private string remotePath;
        private string localPath;

        /// <summary>
        /// 생성자 - App.config에서 FTP 접속정보 로드
        /// </summary>
        public FTPModule()
        {
            try
            {
                host = ConfigurationManager.AppSettings["FTPHost"];
                port = int.Parse(ConfigurationManager.AppSettings["FTPPort"] ?? "21");
                username = ConfigurationManager.AppSettings["FTPUser"];
                password = ConfigurationManager.AppSettings["FTPPassword"];
                remotePath = ConfigurationManager.AppSettings["FTPRemotePath"];
                localPath = ConfigurationManager.AppSettings["DLLLocalPath"];

                // 로컬 캐시 폴더 생성
                if (!string.IsNullOrEmpty(localPath) && !Directory.Exists(localPath))
                {
                    Directory.CreateDirectory(localPath);
                }
            }
            catch (Exception ex)
            {
                cm.writeLog($"FTPModule Init Error: {ex.Message}");
            }
        }

        /// <summary>
        /// FTP 서버에서 DLL 파일을 다운로드합니다.
        /// 항상 최신 버전을 다운로드합니다.
        /// </summary>
        /// <param name="dllFileName">DLL 파일명 (예: FormName.dll)</param>
        /// <returns>로컬 파일 경로. 실패 시 null 반환.</returns>
        public string DownloadDll(string dllFileName)
        {
            try
            {
                string remoteFilePath = remotePath.TrimEnd('/') + "/" + dllFileName;
                string localFilePath = Path.Combine(localPath, dllFileName);

                // 기존 파일이 있으면 삭제 (항상 최신 다운로드)
                if (File.Exists(localFilePath))
                {
                    try
                    {
                        File.Delete(localFilePath);
                    }
                    catch
                    {
                        // 파일이 잠겨있으면 타임스탬프 붙인 이름으로 다운로드
                        string uniqueName = $"{Path.GetFileNameWithoutExtension(dllFileName)}_{DateTime.Now:yyyyMMddHHmmss}.dll";
                        localFilePath = Path.Combine(localPath, uniqueName);
                    }
                }

                // FTP URI 구성
                string ftpUri = $"ftp://{host}:{port}{remoteFilePath}";

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUri);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(username, password);
                request.UseBinary = true;
                request.UsePassive = true;
                request.KeepAlive = false;
                request.Timeout = 30000; // 30초 타임아웃

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (FileStream fs = new FileStream(localFilePath, FileMode.Create))
                {
                    responseStream.CopyTo(fs);
                }

                cm.writeLog($"FTPModule: Downloaded - {dllFileName}");
                return localFilePath;
            }
            catch (WebException wex)
            {
                FtpWebResponse ftpResponse = wex.Response as FtpWebResponse;
                if (ftpResponse != null && ftpResponse.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    cm.writeLog($"FTPModule: Remote file not found - {dllFileName}");
                }
                else
                {
                    cm.writeLog($"FTPModule DownloadDll Error [{dllFileName}]: {wex.Message}");
                }
                return null;
            }
            catch (Exception ex)
            {
                cm.writeLog($"FTPModule DownloadDll Error [{dllFileName}]: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// FTP 연결 테스트
        /// </summary>
        /// <returns>연결 성공 여부</returns>
        public bool TestConnection()
        {
            try
            {
                string ftpUri = $"ftp://{host}:{port}{remotePath}";

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUri);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential(username, password);
                request.UsePassive = true;
                request.Timeout = 10000;

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    return response.StatusCode == FtpStatusCode.OpeningData ||
                           response.StatusCode == FtpStatusCode.DataAlreadyOpen;
                }
            }
            catch (Exception ex)
            {
                cm.writeLog($"FTPModule TestConnection Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 로컬 DLL 캐시 폴더 정리 (앱 시작 시 호출)
        /// </summary>
        public void CleanCache()
        {
            try
            {
                if (Directory.Exists(localPath))
                {
                    foreach (string file in Directory.GetFiles(localPath, "*.dll"))
                    {
                        try { File.Delete(file); }
                        catch { /* 사용 중인 파일은 무시 */ }
                    }
                }
            }
            catch (Exception ex)
            {
                cm.writeLog($"FTPModule CleanCache Error: {ex.Message}");
            }
        }
    }
}
