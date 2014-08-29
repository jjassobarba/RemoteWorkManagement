using System;
using System.Web.Mvc;
using Scio.RemoteManagementModels.Entities;
using Scio.RemoteManagementModels.RepositoriesContracts;

namespace RemoteWorkManagement.Controllers
{
    public class MessagesController : Controller
    {
        private IMessagesRepository _messagesRepository;
        private IUserInfoRepository _userInfoRepository;

        public MessagesController(IMessagesRepository messagesRepository, IUserInfoRepository userInfoRepository)
        {
            _messagesRepository = messagesRepository;
            _userInfoRepository = userInfoRepository;
        }

        //
        // GET: /Messages/
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Inserts the message.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="recipient">The recipient.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public JsonResult InsertMessage(string username,string recipient, string subject, string message)
        {
            
            var messageObject = new Messages()
            {
                Date = DateTime.Now,
                
            };
            return null;
        }
	}
}