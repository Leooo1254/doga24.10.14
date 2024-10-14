using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using GradeApi.Models;
using GradeApi.Data;

namespace GradeApi.Models;
public class Jegy
{
    public int Azon { get; set; }
    public string Jegy { get; set; }
    public string Leiras { get; set; }
    public DateTime Ido { get; set; }
}

[Route("api/[controller]")]
[ApiController]
public class GradesController : ControllerBase
{
    private readonly Connect _dbConnection;

    public GradesController()
    {
        _dbConnection = new Connect();
    }

    // GET: api/grades
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Jegy>>> GetGrades()
    {
        var grades = new List<Jegy>();
        await _dbConnection.Connection.OpenAsync();

        using (var command = new MySqlCommand("SELECT * FROM jegyek", _dbConnection.Connection))
        using (var reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                grades.Add(new Jegy
                {
                    Azon = reader.GetInt32(0),
                    Jegy = reader.GetString(1),
                    Leiras = reader.GetString(2),
                    Ido = reader.GetDateTime(3)
                });
            }
        }

        await _dbConnection.Connection.CloseAsync();
        return grades;
    }

    // GET: api/grades/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Jegy>> GetGrade(int id)
    {
        Jegy grade = null;
        await _dbConnection.Connection.OpenAsync();

        using (var command = new MySqlCommand("SELECT * FROM jegyek WHERE azon = @id", _dbConnection.Connection))
        {
            command.Parameters.AddWithValue("@id", id);
            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    grade = new Jegy
                    {
                        Azon = reader.GetInt32(0),
                        Jegy = reader.GetString(1),
                        Leiras = reader.GetString(2),
                        Ido = reader.GetDateTime(3)
                    };
                }
            }
        }

        await _dbConnection.Connection.CloseAsync();

        if (grade == null)
        {
            return NotFound();
        }

        return grade;
    }

    // POST: api/grades
    [HttpPost]
    public async Task<ActionResult<Jegy>> PostGrade(Jegy grade)
    {
        await _dbConnection.Connection.OpenAsync();

        using (var command = new MySqlCommand("INSERT INTO jegyek (jegy, leiras) VALUES (@jegy, @leiras)", _dbConnection.Connection))
        {
            command.Parameters.AddWithValue("@jegy", grade.Jegy);
            command.Parameters.AddWithValue("@leiras", grade.Leiras);
            await command.ExecuteNonQueryAsync();
            grade.Azon = (int)command.LastInsertedId; // Az utolsó beszúrt azonosító
        }

        await _dbConnection.Connection.CloseAsync();
        return CreatedAtAction(nameof(GetGrade), new { id = grade.Azon }, grade);
    }

    // PUT: api/grades/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutGrade(int id, Jegy grade)
    {
        if (id != grade.Azon)
        {
            return BadRequest();
        }

        await _dbConnection.Connection.OpenAsync();

        using (var command = new MySqlCommand("UPDATE jegyek SET jegy = @jegy, leiras = @leiras WHERE azon = @id", _dbConnection.Connection))
        {
            command.Parameters.AddWithValue("@jegy", grade.Jegy);
            command.Parameters.AddWithValue("@leiras", grade.Leiras);
            command.Parameters.AddWithValue("@id", id);
            await command.ExecuteNonQueryAsync();
        }

        await _dbConnection.Connection.CloseAsync();
        return NoContent();
    }

    // DELETE: api/grades/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGrade(int id)
    {
        await _dbConnection.Connection.OpenAsync();

        using (var command = new MySqlCommand("DELETE FROM jegyek WHERE azon = @id", _dbConnection.Connection))
        {
            command.Parameters.AddWithValue("@id", id);
            await command.ExecuteNonQueryAsync();
        }

        await _dbConnection.Connection.CloseAsync();
        return NoContent();
    }
}
