﻿using BLL.Interfaces;
using Domain;
using Domain.DTO.Requests;
using Domain.DTO.Responses;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace M7BookAPI.Controllers;

/// <summary>
/// Control all Book Store 
/// </summary>
public class BookStoreController : APIBaseController
{
    private readonly IBookStoreService _bookStoreService;

    public BookStoreController(IBookStoreService bookStoreService)
    {
        _bookStoreService = bookStoreService;       
    }

    /// <summary>
    /// Get all Books
    /// </summary>
    /// <returns>Returns all books on API</returns>
    [HttpGet("books")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
       var username = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
       return Ok(await _bookStoreService.GetBooksAsync());
    }

    /// <summary>
    /// Get a Book by unique identifier
    /// </summary>
    /// <param name="id">Unique identifier</param>
    /// <returns></returns>
    [HttpGet("books/{id}")]
    public async Task<IActionResult> GetBook(int id)
    {
        if(id <= 0) return BadRequest(); //1

        var book = await _bookStoreService.GetBookByIdAsync(id);

        //DTO Response
        var response = new BookResponse
        {
            Id = book.Id,
            ISBN = book.ISBN,
            Titre = book.Title,
            Description = book.Description,
            AuthorId = book.AuthorId
        };
        return Ok(response); //3
    }

    /// <summary>
    /// Add a new book on API
    /// </summary>
    /// <param name="book">Book to Add</param>
    /// <returns></returns>
    [HttpPost("books")]
    public async Task<IActionResult> Post([FromBody] CreateBookRequest CreateBookRequest)
    {
        var book = new Book()
        {
            AuthorId = CreateBookRequest.AuthorId.GetValueOrDefault(),
            Description = CreateBookRequest.Description,
            ISBN = CreateBookRequest.ISBN,
            Title = CreateBookRequest.Title
        };

        //business logic
        var result = await _bookStoreService.AddBookAsync(book);
        //DTO Response
        var response = new CreateBookResponse
        {
            Id = result.Id,
            ISBN = result.ISBN,
            Titre = result.Title,
            Description = result.Description,
            AuthorId = result.AuthorId
        };


        return CreatedAtAction(nameof(GetBook),new {id = response.Id}, response);
    }

    /// <summary>
    /// Update a book on API
    /// </summary>
    /// <param name="book"></param>
    /// <returns></returns>
    [HttpPut("books/{idBook}")]

    public Task<IActionResult> Put([FromRoute] int idBook, [FromBody] ModifyBookRequest ModifyBookRequest)
    {
        return Task.FromResult<IActionResult>(Ok());
    }

}
