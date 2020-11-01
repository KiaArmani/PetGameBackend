using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using PetGameBackend.Models.Requests.Pet;
using PetGameBackend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace PetGameBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PetController : ControllerBase
    {
        private readonly PetService _petService;

        public PetController(PetService petService)
        {
            _petService = petService;
        }

        // GET api/<PetController>
        /// <summary>
        ///     Returns information regarding a pet.
        /// </summary>
        /// <response code="200">When the pet was found</response>
        /// <response code="400">When the payload is missing or the pet identifier is ambiguous</response>
        /// <response code="404">When the pet couldn't be found</response>
        /// <response code="422">When the pet identifier couldn't be parsed as GUID</response>
        /// <response code="500">When any other unhandled exception</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult Get([FromBody] PetControllerRootGet payload)
        {
            // Check if payload is present
            if (payload == null)
                return BadRequest("PetController (GET) - Missing payload.");

            try
            {
                // Get user from user service and return it
                var pet = _petService.GetPet(payload);
                if (pet == null)
                    return NotFound("PetController (GET) - User not found.");

                return Content(JsonConvert.SerializeObject(pet));
            }
            catch (InvalidCastException e)
            {
                // Return 422 if we can't parse the pet identifier as GUID
                return UnprocessableEntity(e.Message);
            }
            catch (InvalidDataException e)
            {
                // Return 400 if the payload contains invalid values
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                // Return 500 if any other exception occurred
                return Problem(e.Message, e.Source, 500, "PetController (GET)", e.GetType().ToString());
            }
        }

        // POST api/<PetController>
        /// <summary>
        ///     Creates a new pet for the given user
        ///     If the pet already exists, no new pet will be created
        /// </summary>
        /// <response code="200">When the pet was crated</response>
        /// <response code="404">When the pet couldn't be found</response>
        /// <response code="409">When the given user doesn't exist</response>
        /// <response code="422">When the payload couldn't be parsed as JSON or an identifier couldn't be parsed as GUID</response>
        /// <response code="500">When any other unhandled exception occurs</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult Post([FromBody] PetControllerRootPost payload)
        {
            // Check if payload is present
            if (payload == null)
                return BadRequest("PetController (POST) - Missing payload.");

            try
            {
                // Request user to be created
                var petIdentifier = _petService.CreatePet(payload);
                return Ok(petIdentifier);
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
        }

        // DELETE api/<PetController>/
        /// <summary>
        ///     Deletes a given user
        /// </summary>
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
        public ActionResult Delete([FromBody] PetControllerRootDelete payload)
        {
            // Check if payload is present
            if (payload == null)
                return BadRequest("PetController (DELETE) - Missing payload.");

            try
            {
                _petService.DeletePet(payload);
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