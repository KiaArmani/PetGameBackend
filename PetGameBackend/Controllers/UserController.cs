using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using PetGameBackend.Models.Requests.User;
using PetGameBackend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace PetGameBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        // GET api/<UserController>/5
        /// <summary>
        ///     Returns information regarding a user.
        /// </summary>
        /// <param name="payload">Payload (<see cref="UserControllerRootGet" />)</param>
        /// <response code="200">When the user was found</response>
        /// <response code="400">When the payload is missing or the user identifier is ambiguous</response>
        /// <response code="404">When the user couldn't be found</response>
        /// <response code="422">When the user identifier couldn't be parsed as GUID</response>
        /// <response code="500">When any other unhandled exception</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult Get([FromBody] UserControllerRootGet payload)
        {
            // Check if payload is present
            if (payload == null)
                return BadRequest("UserController (GET) - Missing payload.");

            try
            {
                // Get user from user service and return it
                var user = _userService.GetUser(payload);
                if (user == null)
                    return NotFound("UserController (GET) - User not found.");

                return Content(JsonConvert.SerializeObject(user));
            }
            catch (InvalidCastException e)
            {
                // Return 422 if we can't parse the user identifier as GUID
                return UnprocessableEntity(e.Message);
            }
            catch (InvalidDataException e)
            {
                // Return 400 if the user identifier is ambiguous
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                // Return 500 if any other exception occurred
                return Problem(e.Message, e.Source, 500, "UserController (GET)", e.GetType().ToString());
            }
        }

        // POST api/<UserController>/
        /// <summary>
        ///     Creates a new user with the given user identifier
        ///     If the user already exists, no new user will be created
        /// </summary>
        /// <param name="payload">Payload(<see cref="UserControllerRootPost" />)</param>
        /// <response code="200">When the user was crated</response>
        /// <response code="404">When the user couldn't be found</response>
        /// <response code="409">When a user with the given GUID already exists</response>
        /// <response code="422">When the payload couldn't be parsed as JSON or user identifier couldn't be parsed as GUID</response>
        /// <response code="500">When any other unhandled exception occurs</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult Post([FromBody] UserControllerRootPost payload)
        {
            // Check if payload is present
            if (payload == null)
                return BadRequest("UserController (POST) - Missing payload.");

            try
            {
                // Request user to be created
                _userService.CreateUser(payload);
            }
            catch (JsonSerializationException e)
            {
                // Return 422 if we can't parse the payload as JSON
                return UnprocessableEntity(e.Message);
            }
            catch (InvalidCastException e)
            {
                // Return 422 if we can't parse the user identifier as GUID
                return UnprocessableEntity(e.Message);
            }
            catch (InvalidOperationException e)
            {
                // Return 409 if the user already exists
                return Conflict(e.Message);
            }
            catch (Exception e)
            {
                // Return 500 if any other exception occurred
                return Problem(e.Message, e.Source, 500, "UserController (POST)", e.GetType().ToString());
            }

            return Ok();
        }

        // PUT api/<UserController>/
        /// <summary>
        ///     Updates the provided user
        /// </summary>
        /// <param name="payload">Payload (<see cref="UserControllerRootPut" />)</param>
        /// <response code="200">When the user was updated</response>
        /// <response code="400">When the payload doesn't contain any user identifier or pet data, or when no updates were performed</response>
        /// <response code="404">When the user couldn't be found</response>
        /// <response code="409">When more than one update was performed</response>
        /// <response code="422">When the payload couldn't be parsed as JSON or user identifier couldn't be parse as GUID</response>
        /// <response code="500">When any other unhandled exception occurs</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult Put([FromBody] UserControllerRootPut payload)
        {
            // Check if payload is present
            if (payload == null)
                return BadRequest("UserController (POST) - Missing payload.");

            try
            {
                _userService.UpdateUser(payload);
                return Ok();
            }
            catch (JsonSerializationException e)
            {
                // Return 422 if we can't parse the payload as JSON
                return UnprocessableEntity(e.Message);
            }
            catch (InvalidDataException e)
            {
                // Return 400 if the payload doesn't contain any user identifier
                return BadRequest(e.Message);
            }
            catch (NoNullAllowedException e)
            {
                // Return 400 if no updates were performed
                return BadRequest(e.Message);
            }
            catch (KeyNotFoundException e)
            {
                // Return 404 if we couldn't find the user we wanted to update
                return NotFound(e.Message);
            }
            catch (InvalidOperationException e)
            {
                // Return 409 if more than one update was performed
                return Conflict(e.Message);
            }
            catch (Exception e)
            {
                // Return 500 if any other exception occurred
                return Problem(e.Message, e.Source, 500, "UserController (PUT)", e.GetType().ToString());
            }
        }

        // DELETE api/<UserController>/
        /// <summary>
        ///     Deletes a given user
        /// </summary>
        /// <param name="payload">Payload (<see cref="UserControllerRootDelete" />)</param>
        /// <response code="200">When the user was updated</response>
        /// <response code="400">When the user identifier is ambiguous</response>
        /// <response code="404">When the user couldn't be found</response>
        /// <response code="409">When more than one deletion was performed</response>
        /// <response code="422">When the user identifier couldn't be parsed as GUID</response>
        /// <response code="500">When any other unhandled exception occurs</response>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult Delete([FromBody] UserControllerRootDelete payload)
        {
            // Check if payload is present
            if (payload == null)
                return BadRequest("UserController (POST) - Missing payload.");

            try
            {
                _userService.DeleteUser(payload);
                return Ok();
            }
            catch (InvalidCastException e)
            {
                // Return 422 if we can't parse the user identifier as GUID
                return UnprocessableEntity(e.Message);
            }
            catch (InvalidDataException e)
            {
                // Return 400 if the user identifier is ambiguous
                return BadRequest(e.Message);
            }
            catch (InvalidOperationException e)
            {
                // Return 409 if more than one deletion was performed
                return Conflict(e.Message);
            }
            catch (NoNullAllowedException e)
            {
                // Return 400 if no deletion was performed
                return BadRequest(e.Message);
            }
            catch (KeyNotFoundException e)
            {
                // Return 404 if we couldn't find the user we wanted to delete
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                // Return 500 if any other exception occurred
                return Problem(e.Message, e.Source, 500, "UserController (DELETE)", e.GetType().ToString());
            }
        }
    }
}