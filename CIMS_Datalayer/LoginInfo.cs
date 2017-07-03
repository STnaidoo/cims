using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.Configuration;

namespace CIMS_Datalayer
{
    public class LoginInfo
    {
        CIMS_Entities _db = new CIMS_Entities();
        ErrorLogging erl = new ErrorLogging();


        public system_users GetSystemUser(string system_User_Login)
        {
            try
            {
                var users = (from data in _db.system_users
                             where data.system_user_login == system_User_Login
                             select data).FirstOrDefault();

                return users;
            }
            catch(Exception ex)
            {
                erl.LogError("GetAllSystemUsers", "Message: " + ex.Message + ", inner exception: " + ex.InnerException.ToString());
                throw ex;
            }
        }


        public bool Check_Password(string user_name, string db_password, string provided_password, string login_option)
        {
            try
            {
                if (login_option.ToLower() == "ldap")
                {
                    if (ldap_validation(user_name, provided_password))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (db_password == provided_password)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                erl.LogError("Check_Password", ex.Message);
                throw ex;
            }

        }

        private bool ldap_validation(string user_name_in, string user_pass_in)
        {
            
            DirectoryEntry entry = new DirectoryEntry(("LDAP://" + ConfigurationManager.AppSettings["ldap_domain"]), user_name_in, user_pass_in);

            DirectorySearcher searcher = new DirectorySearcher(entry);
            searcher.SearchScope = SearchScope.OneLevel;

            try
            {

                SearchResult results = searcher.FindOne();
                if (results != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("ldap_validation", ex.Message);
                return false;
            }              
        }

        public void UpdateFailedLoginCounter(int system_user_id, int? failedLoginCounter)
        {
            try
            {
                system_users user = (from us in _db.system_users
                                     where us.system_user_id == system_user_id
                                     select us).First();

                if (failedLoginCounter == null)
                {
                    user.failed_login_count = 1;
                }
                else
                {
                    user.failed_login_count = failedLoginCounter + 1;
                }

                _db.SaveChanges();

            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("UpdateFailedLoginCounter", ex.Message);
            }
        }

        public user_type UserTypeDetails(string system_user_type)
        {
            try
            {
                var item = (from data in _db.user_type
                            where data.user_type_no == system_user_type
                            select data).FirstOrDefault();

                return item;
            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("UserTypeDetails", ex.Message);
                throw ex;
            }
        }

        public string GetUserTitle(int? system_user_title)
        {
            try
            {
                var item = (from data in _db.user_title
                            where data.title_id == system_user_title
                            select data.title_name).FirstOrDefault();

                return item;
            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("getUserTitle", ex.Message);
                throw ex;
            }
        }

        public string GetUserBranch(int? system_user_branch)
        {
            try
            {
                var item = (from data in _db.user_branch
                            where data.branch_id == system_user_branch
                            select data.branch_name).FirstOrDefault();

                return item;
            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("getUserBranch", ex.Message);
                throw ex;
            }
        }

        public void UpdatePassword(string system_user_login,string password)
        {
            try
            {
                system_users user = (from us in _db.system_users
                                     where us.system_user_login == system_user_login
                                     select us).First();
               
                    user.system_user_password = password;               

                _db.SaveChanges();

            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("UpdatePassword", ex.Message);
            }
        }

        public string GetDuplicateDays(string settingName)
        {
            try
            {
                var item = (from us in _db.system_settings
                                     where us.setting_name == settingName
                            select us.setting_value).First();

                return item;

            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("GetDuplicateDays", ex.Message);
                throw ex;
            }
        }

    }
}