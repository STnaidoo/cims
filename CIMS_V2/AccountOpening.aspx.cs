using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data.Common;
using CIMS_Datalayer;
using CIMS_V2.AddOn;

namespace CIMS_V2
{
    public partial class AccountOpening : System.Web.UI.Page
    {
        CIMS_Entities _db = new CIMS_Entities();
        GenericDbFunctions genericFunctions = new GenericDbFunctions();
        GetNextInfo getNextInfo = new GetNextInfo();
        Alerts alert = new Alerts();
        SharedFunctions sharedUtility = new SharedFunctions();
        ErrorLogging erl = new ErrorLogging();
        OperationsLog operationsLog = new OperationsLog();
        RijndaelEnhanced rienha;
        DAccessInfo daccess = new DAccessInfo();
        Constants constants = new Constants();

        ErrorLogging logError = new ErrorLogging();
        ToolSet tool = new ToolSet();

        protected void Page_Init(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Expires = 0;

            try
            {
                if (string.IsNullOrEmpty(Session["UserID"].ToString()))
                {
                    Server.Transfer("~/Account/Login.aspx", false);
                }
            }
            catch (Exception ex)
            {
                logError.LogError("AccountOpening", ex.StackTrace);
                Server.Transfer("~/Account/Login.aspx", false);
            }


            /* Commented out for debugging purposes
            if (!Session["UserType"].Equals("8"))

            {
                Server.Transfer("No_rights.aspx", false);
            }
            */
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session.Contents.Count == 0)
            {
                Response.Redirect("~/Account/Login.aspx");
            }

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Expires = 0;

            try
            {
                if (Session["UserID"].ToString() == null || Session["UserID"].ToString() == "")
                {
                    Response.Redirect("Login.aspx");
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("Login.aspx");
            }

            if (Session["UserType"].ToString() != "1")
            {
                Response.Redirect("no_rights.aspx");
            }
            else
            {
                if (!IsPostBack)
                {
                    LoadPage();
                }
            }
        }

        private void LoadPage()
        {
            sharedUtility.LoadDropDownList(drpDocumentStatus, genericFunctions.GetDropdownListInfo("document_status", new string[] { "document_status_id", "document_status_action" }, new string[] { "document_status_user_type_who_can_action", "(is_archive is null OR is_archive" }, new string[] { Session["UserType"].ToString(), "0)" }), "document_status_action", "document_status_id");
            sharedUtility.LoadDropDownList(drpBranches, genericFunctions.GetDropdownListInfo("user_branch", new string[] { "branch_id", "branch_name" }, null, null), "branch_name", "branch_id");


        }



        protected void btnAttach_Click(object sender, EventArgs e)
        {
            if(pdfUpload.HasFile && pdfUpload.FileName.ToLower().Contains(".pdf"))
            {
                string fl_name = DateTime.Now.ToString("dd_MMM_yyyy_hh_mm_ss") + pdfUpload.FileName.Replace(" ", "_");
                string path = Server.MapPath("~") + "instructions/" + fl_name;
                path = path.Replace("\\", "/");
                pdfUpload.SaveAs(path);
                pdfViewer.FilePath = "instructions/" + fl_name;
                txtUploadedPDF.Text = pdfUpload.FileName;

                try
                {
                    instruction ins = new instruction();

                    ins.file_name = fl_name;
                    ins.instruction_type_id = 13; //account opening
                    ins.reference = "ACO/" + DateTime.Now + "/" + ins.instruction_id;
                    txtReference.Text = ins.reference;
                    int inserted_by = Convert.ToInt32(Session["UserID"]);
                    ins.inserted_by = inserted_by;
                    int modified_by = Convert.ToInt32(Session["UserID"]);
                    DateTime inserted_date = Convert.ToDateTime("1/1/1900");

                    _db.SaveChanges();

                    
                }
                catch(Exception ex)
                {
                    alert.FireAlerts(this.Page, "Error creating instruction " + ex.ToString());
                }


            }
            else
            {
                alert.FireAlerts(this.Page, "Please select a pdf file");
            }
        
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                instruction ins = _db.instructions.FirstOrDefault(i => i.reference == txtReference.Text);
                ins.branch_id = Convert.ToInt32(drpBranches.SelectedValue);
                ins.document_status_id = Convert.ToInt32(drpDocumentStatus.SelectedValue);

                _db.SaveChanges();

                alert.FireAlerts(this.Page, "Instruction submitted");
                //get next user

            }
            catch(Exception ex)
            {
                alert.FireAlerts(this.Page, "Error submitting instruction " + ex.ToString());
            }
           

        }
        public int GetUserWithLeastWork(int userTypeId, int instructionTypeId)
        {
            int assignedUserId = 0;

            //get branchcode based on originator branch code
            string branchId = (Session["UserBranchID"].ToString());

            int branchIdVal = Convert.ToInt32(branchId);
            var branchCode = _db.user_branch.FirstOrDefault(b => b.branch_id == branchIdVal).branch_code;

            //get all the allocations (records where a system_user ID is allocated to an instruction type)
            IQueryable<instruction_type_allocations> allocations = from allo in _db.instruction_type_allocations
                                                                   where allo.instruction_type_id == instructionTypeId &&
                                                                   allo.system_users.system_user_type == userTypeId &&
                                                                   allo.status == 1
                                                                   select allo;

            //get all ACTIVE users with the requested USERTYPE and BRANCH CODE 
            //List those users firstly by TOTALALLOCATEDWORK and secondly by ID
            IQueryable<system_users> userList = (from users in _db.system_users
                                                 where users.system_user_type == userTypeId &&
                                                 users.system_user_branch_code == branchCode.ToString() &&
                                                 users.system_user_active == 1
                                                 select users).OrderBy(u => u.total_work_allocated).ThenByDescending(u => u.system_user_id);


            //if there aren't any available users from that branch
            if (userList.Count() == 0)
            {
                userList = (from users in _db.system_users
                            where users.system_user_type == userTypeId &&
                            users.system_user_active == 1
                            select users).OrderBy(u => u.total_work_allocated).ThenByDescending(u => u.system_user_id);
            }
            //remove the current user from the userList because you don't want to end up sending to yourself RIP segregation of duties

            List<system_users> userListLessLoggedInUser = new List<system_users>();
            assignedUserId = Convert.ToInt32(Session["UserID"].ToString());

            if (userList.Count() > 0)
            {
                foreach (system_users user in userList)
                {
                    if (user.system_user_id != assignedUserId)
                    {
                        userListLessLoggedInUser.Add(user);
                    }
                }
            }
            List<system_users> allocatedUserList = new List<system_users>();

            //now compare the list of users who are active and of the right type with users with the right instruction type allocation. and make a list with the ones that match.
            if (userListLessLoggedInUser.Count() > 0)
            {
                foreach (system_users user in userListLessLoggedInUser)
                {
                    long userId = user.system_user_id; //get the user's ID
                    long acceptedUserId = 0;
                    if (allocations.FirstOrDefault(a => a.system_user_id == userId) != null)
                    {
                        acceptedUserId = (long)allocations.FirstOrDefault(a => a.system_user_id == userId).system_user_id; //check if the user is allocated to this instruction type
                    }
                    if (acceptedUserId == userId)
                    {
                        //add the allocated, active user of the correct user type to the new list of users
                        allocatedUserList.Add(userListLessLoggedInUser.FirstOrDefault(u => u.system_user_id == acceptedUserId));
                    }
                }
            }
            else
            {
                foreach (system_users user in userList)
                {
                    long userId = user.system_user_id; //get the user's ID
                    long acceptedUserId = 0;
                    if (allocations.FirstOrDefault(a => a.system_user_id == userId) != null)
                    {
                        acceptedUserId = (long)allocations.FirstOrDefault(a => a.system_user_id == userId).system_user_id; //check if the user is allocated to this instruction type
                    }
                    if (acceptedUserId == userId)
                    {
                        //add the allocated, active user of the correct user type to the new list of users
                        allocatedUserList.Add(userList.FirstOrDefault(u => u.system_user_id == acceptedUserId));
                    }
                }
            }


            //count of how many instructions are currently allocated to a user
            int allocatedWorkCount = 0;


            int maxTotalWork = (int)allocatedUserList.Max(u => u.total_work_allocated);

            //get a list of all the instructions
            var instructions = from ins in _db.instructions
                               select ins;

            int lowestAllocation = instructions.Count(); //You can't be allocated more work than there are instructions

            //loop through the list to find the user with the least to do or just give work to the first person with nothing to do. 



            //select all users with the lowest allocated work count (from instructions list) and lowest total work count attribute
            List<int> workCountList = new List<int>();

            foreach (system_users user in allocatedUserList) //users are sorted by historical work count
            {
                int count;
                //get all the allocated work counts into a list so we can find the minimum.
                count = instructions.Count(i => i.allocated_to == user.system_user_id);
                workCountList.Add(allocatedWorkCount);
            }
            //refine the list for those users with the lowest total work allocated
            List<system_users> lowestAllocationUsers = new List<system_users>();

            //add all the users with the lowest allocated work counts to the list
            foreach (system_users user in allocatedUserList)
            {

                //get the count for the current user in the outer loop
                allocatedWorkCount = instructions.Count(i => i.allocated_to == user.system_user_id);

                //then compare it with the list of work counts
                if (allocatedWorkCount == workCountList.Min()) //add the user if their allocated workcount is the minimum. 
                {
                    lowestAllocationUsers.Add(user);
                }
            }

            int minTotalWork = (int)allocatedUserList.Min(u => u.total_work_allocated);

            //if historical total work is higher in the lowest allocation users than in the bigger allocated user list, use the minimum of the lowest allocation users
            if (lowestAllocationUsers.Min(u => u.total_work_allocated) >= allocatedUserList.Min(u => u.total_work_allocated))
            {
                minTotalWork = (int)lowestAllocationUsers.Min(u => u.total_work_allocated);
            }
            //from the users with lowest allocated work, get the users with the lowest historical allocated work
            var usersWithLeastTotalWork = lowestAllocationUsers.ToList().Where(u => u.total_work_allocated == minTotalWork);

            //select the first user in this list as the assigned user for the instruction. 
            if (usersWithLeastTotalWork.Count() > 0)
            {
                assignedUserId = (int)usersWithLeastTotalWork.FirstOrDefault().system_user_id;

            }
            else if (lowestAllocationUsers.Count() > 0)
            {
                assignedUserId = (int)lowestAllocationUsers.FirstOrDefault().system_user_id;
            }
            else if (allocatedUserList.Count() > 0)
            {
                assignedUserId = (int)allocatedUserList.FirstOrDefault().system_user_id;
            }
            else
            {
                //if there isn't anyone left to assign to, assign the instruction to the current logged in user. :(
                assignedUserId = Convert.ToInt32(Session["UserID"].ToString());
                alert.FireAlerts(this.Page, "Couldn't find an acceptable user to assign to. Please try again later.");
            }

            return assignedUserId;

        }






    }
}