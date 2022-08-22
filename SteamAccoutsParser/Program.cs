using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;

namespace SteamAccoutsParser
{
    internal class Program
    {
        public class User
        {

          
            public string SteamID { get; set; }
          
            public string AccountName { get; set; }
            public string PersonaName { get; set; }
            public string RememberPassword { get; set; }
            public string WantsOfflineMode { get; set; }
            public string SkipOfflineModeWarning { get; set; }
            public string AllowAutoLogin { get; set; }
            public string MostRecent { get; set; }
            public string Timestamp { get; set; }

            public override string ToString()
            {
                return $"${PersonaName} {SteamID} {AccountName}";
            }

        }

        static void Main(string[] args)
        {
            //string path_ = @"E:\Steam\config\loginusers.vdf";
            string path_ = @"";
            try
            {
                RegistryKey currentUserKey = Registry.CurrentUser;
                RegistryKey steam = currentUserKey.OpenSubKey("Software\\Valve\\Steam");
                path_ = Path.Combine( (string)steam.GetValue("SteamPath") , "config\\loginusers.vdf");
                while (!File.Exists(path_))
                {
                    path_ = Console.ReadLine();
                }
                Console.WriteLine(path_);
            }
            catch (Exception)
            {             
            }
           

            List<User> users = new List<User>();
           
            string data_ = File.ReadAllText(path_);
            string steamid_ = "";
            foreach (Match item in Regex.Matches(data_, "\\\"[0-9]+\\\"[\\s\\S]+?{[\\w\\W]+?}"))
            {
                foreach (Match steamid in Regex.Matches(item.Value, "\"[0-9]+\""))
                {

                    if (steamid.Value.Length > 10)
                    {
                        steamid_ = steamid.Value.Replace("\"", "");
                        break;
                    }
                }
                //  Console.WriteLine(steamid_);
                //Console.WriteLine(item.Value);
                foreach (Match user in Regex.Matches(item.Value, "{[\\w\\W]+?}"))
                {
                    Console.WriteLine($"=====\n");
                    User user_ = new User()
                    {
                        SteamID = steamid_,
                    };
                    int a = 0;
                    foreach (Match user_data in Regex.Matches(user.Value, "\\\"[\\w]+\\\"[ \\W]+?\\\"[\\w]+\\\""))
                    {



                        int v = 0;
                        string name_p = "";
                        string name_va = "";
                        foreach (Match value in Regex.Matches(user_data.Value.Trim(), @"[\w]+"))
                        {
                            if (v == 0)
                                name_p = value.Value;
                            if (v == 1)
                                name_va = value.Value;
                            v++;
                        }
                        switch (name_p)
                        {
                            case "AccountName":
                                user_.AccountName = name_va;
                                break;
                          
                            case "PersonaName":
                                user_.PersonaName = name_va;
                                break;
                            case "Timestamp":
                                user_.Timestamp = name_va;
                                break;
                        }

                    }
                    users.Add(user_);
                }




            }
            foreach (var item in users)
            {
                Console.WriteLine(item);
            }

            Console.ReadLine();
        }
    }
}
