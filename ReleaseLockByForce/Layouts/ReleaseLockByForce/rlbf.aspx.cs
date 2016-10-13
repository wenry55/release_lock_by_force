using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

// this is a page for "Release Lock By Force", aka rlbf.

namespace ReleaseLockByForce.Layouts.ReleaseLockByForce
{

    public partial class rlbf : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {
                if (Request["lockedFile"] == null)
                {
                    throw new Exception("no locked filed specified.");
                }

                string fileUrl = Request["lockedFile"];

                using (SPSite site = new SPSite(fileUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        var fileURL = fileUrl.Replace(site.Url + "/", string.Empty);
                        var lockedFile = web.GetFile(fileURL);
                        var lockedUserID = lockedFile.LockedByUser.ID;
                        var lockedUser = web.AllUsers.GetByID(lockedUserID);
                        using (SPSite userSite = new SPSite(fileUrl, lockedUser.UserToken))
                        {
                            using (SPWeb userWeb = userSite.OpenWeb())
                            {
                                userWeb.AllowUnsafeUpdates = true;
                                lockedFile = userWeb.GetFile(fileUrl);
                                lockedFile.ReleaseLock(lockedFile.LockId);
                                userWeb.AllowUnsafeUpdates = false;
                            }
                        }
                    }
                }

                Response.Write("Success");
            }
            catch (Exception ex)
            {
                Response.Write("Failed:" +ex.Message);
            }
            finally
            {
                Response.End();
            }

            
            

        }
    }
}
