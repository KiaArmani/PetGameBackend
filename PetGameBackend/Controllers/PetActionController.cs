using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using PetGameBackend.Models.Requests.PetAction;
using PetGameBackend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace PetGameBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PetActionController : ControllerBase
    {
        private readonly PetService _petService;

        public PetActionController(PetService petService)
        {
            _petService = petService;
        }

        // PATCH api/<PetActionController>/stroke
        /// <summary>
        ///     Strokes a pet.
        /// </summary>
        /// <response code="200">When the pet was stroke</response>
        /// <response code="400">When the payload doesn't contain any pet identifier or stat value, or when no updates were performed</response>
        /// <response code="404">When the pet couldn't be found</response>
        /// <response code="409">When more than one update was performed</response>
        /// <response code="422">When the payload couldn't be parsed or pet identifier couldn't be parsed as GUID</response>
        /// <response code="500">When any other unhandled exception occurs</response>
        [Route("stroke")]
        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult StrokePet([FromBody] PetActionControllerStrokePatch payload)
        {
            // Check if payload is present
            if (payload == null)
                return BadRequest("PetActionController (PATCH, stroke) - Missing payload.");

            try
            {
                _petService.StrokePet(payload);
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
                // Return 404 if we couldn't find the pet we wanted to update
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

        // PATCH api/<PetActionController>/feed
        /// <summary>
        ///     Feeds a pet.
        /// </summary>
        /// <response code="200">When the pet was fed</response>
        /// <response code="400">When the payload doesn't contain any pet identifier or stat value, or when no updates were performed</response>
        /// <response code="404">When the pet couldn't be found</response>
        /// <response code="409">When more than one update was performed</response>
        /// <response code="422">When the payload couldn't be parsed or pet identifier couldn't be parsed as GUID</response>
        /// <response code="500">When any other unhandled exception occurs</response>
        [Route("feed")]
        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult FeedPet([FromBody] PetActionControllerFeedPatch payload)
        {
            // Check if payload is present
            if (payload == null)
                return BadRequest("PetActionController (PATCH, feed) - Missing payload.");

            try
            {
                _petService.FeedPet(payload);
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
                // Return 404 if we couldn't find the pet we wanted to update
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
    }
}