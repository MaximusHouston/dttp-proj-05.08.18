using DPO.Common;
using DPO.Model.Light;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPO.Domain 
{
    public interface IFinaliseModelService
    {
        void FinaliseOrderModel(Messages messages, UserSessionModel admin, OrderViewModelLight model);
        void FinaliseOrderModel(UserSessionModel admin, OrderViewModel model);
        //void FinaliseProjectModel(Messages messages, UserSessionModel admin, ProjectModel model);
        //void FinaliseProjectModel(Messages messages, UserSessionModel user, ProjectsModel model, HtmlServices htmlService);
        //void FinaliseProjectModel(Messages messages, UserSessionModel user, ProjectsGridViewModel model, HtmlServices htmlService);
    }
}
