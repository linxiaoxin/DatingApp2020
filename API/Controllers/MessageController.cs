using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTO;
using API.Entities;
using API.Extensions;
using API.Helper;
using API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class MessageController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IRespositories _mainRespositories;
        public MessageController(IRespositories mainRespositories, IMapper mapper)
        {
            _mainRespositories = mainRespositories;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDTO>> AddMessage(CreateMessageDTO message)
        {
            var sendUserName = User.GetUserName();
            if (sendUserName == message.RecipientUserName) return BadRequest("Not allowed to send message to yourself");
            var sender = await _mainRespositories.AppUserRespository.GetUserByUserNameAsync(sendUserName);
            var recipient = await _mainRespositories.AppUserRespository.GetUserByUserNameAsync(message.RecipientUserName);

            if (recipient == null) return NotFound();
            var newMsg = new Message
            {
                SenderId = sender.Id,
                SenderUserName = sender.UserName,
                RecipientId = recipient.Id,
                RecipientUserName = recipient.UserName,
                Content = message.Content
            };
            await _mainRespositories.MessageRepository.AddMessage(newMsg);
            if (await _mainRespositories.Complete()) return Ok(_mapper.Map<MessageDTO>(newMsg));
            return BadRequest("Fail to send message");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageForUser([FromQuery] MessageParams msgParams)
        {
            msgParams.UserName = User.GetUserName();
            var msg = await _mainRespositories.MessageRepository.GetMessageByUserName(msgParams);
            Response.addPaginationHeader(msg.CurrentPage, msg.PageSize, msg.TotalCount, msg.TotalPages);
            return Ok(msg);
        }

        [HttpGet("thread/{otherUserName}")]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageThread(string otherUserName)
        {
            var thisUserName = User.GetUserName();
            var result = await _mainRespositories.MessageRepository.GetMessageThread(thisUserName, otherUserName);
            if(_mainRespositories.HasChanges()) await _mainRespositories.Complete();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUserName();
            var message = await _mainRespositories.MessageRepository.FindMessage(id);

            if (message == null) NotFound();

            if (message.SenderUserName != username && message.RecipientUserName != username) Unauthorized();

            if (message.SenderUserName == username) message.SenderDeleted = true;
            if (message.RecipientUserName == username) message.RecipientDeleted = true;

            if (message.SenderDeleted && message.RecipientDeleted) _mainRespositories.MessageRepository.DeleteMessage(message);

            if (await _mainRespositories.Complete()) return Ok();

            return BadRequest();
        }
    }
}