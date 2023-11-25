namespace Egw.PubManagement.Persistence.Entities;

/// <summary>
/// Publication author
/// </summary>
public class PublicationAuthor : ITimeStampedEntity
{
    /// <summary>
    /// Unique author ID
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// First name
    /// </summary>
    public string FirstName { get; private set; }

    /// <summary>
    /// Middle name
    /// </summary>
    public string MiddleName { get; private set; }

    /// <summary>
    /// Last name
    /// </summary>
    public string LastName { get; private set; }

    /// <summary>
    /// Author code
    /// </summary>
    public string? Code { get; private set; }

    /// <summary>
    /// Author biography
    /// </summary>
    public string? Biography { get; private set; }

    /// <summary>
    /// Birth year
    /// </summary>
    public int? BirthYear { get; private set; }

    /// <summary>
    /// Death year
    /// </summary>
    public int? DeathYear { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset UpdatedAt { get; private set; }


    /// <summary>
    /// Full name
    /// </summary>
    public string FullName
    {
        get
        {
            var b = new List<string>();
            if (!string.IsNullOrEmpty(FirstName))
                b.Add(FirstName);
            if (!string.IsNullOrEmpty(MiddleName))
                b.Add(MiddleName);
            if (!string.IsNullOrEmpty(LastName))
                b.Add(LastName);
            return string.Join(" ", b);
        }
    }

    /// <summary>
    /// Short name
    /// </summary>
    public string ShortName
    {
        get
        {
            string s = LastName;

            if (string.IsNullOrWhiteSpace(FirstName))
                return s;
            s += $", {FirstName[0]}.";

            if (string.IsNullOrWhiteSpace(MiddleName))
                return s;
            s += $" {MiddleName[0]}.";
            return s;
        }
    }

    /// <summary>
    /// Publication author
    /// </summary>
    /// <param name="id">Author ID</param>
    /// <param name="firstName">First name</param>
    /// <param name="middleName">Middle name</param>
    /// <param name="lastName">Last name</param>
    /// <param name="createdAt">Date of creation</param>
    public PublicationAuthor(
        int id,
        string firstName,
        string middleName,
        string lastName,
        DateTimeOffset createdAt)
    {
        Id = id;
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    /// <summary>
    /// Publication author
    /// </summary>
    /// <param name="firstName">First name</param>
    /// <param name="middleName">Middle name</param>
    /// <param name="lastName">Last name</param>
    /// <param name="createdAt">Date of creation</param>
    public PublicationAuthor(string firstName,
        string middleName,
        string lastName,
        DateTimeOffset createdAt)
    {
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    /// <summary>
    /// Updates author code
    /// </summary>
    /// <param name="code">New code</param>
    /// <param name="updatedAt">Date of update</param>
    /// <returns></returns>
    public PublicationAuthor SetCode(string? code, DateTimeOffset updatedAt)
    {
        Code = string.IsNullOrWhiteSpace(code) ? null : code;
        UpdatedAt = updatedAt;
        return this;
    }

    /// <summary>
    /// Updates author biography
    /// </summary>
    /// <param name="biography">New biography</param>
    /// <param name="updatedAt">Date of update</param>
    /// <returns></returns>
    public PublicationAuthor SetBiography(string? biography, DateTimeOffset updatedAt)
    {
        Biography = string.IsNullOrWhiteSpace(biography) ? null : biography;
        UpdatedAt = updatedAt;
        return this;
    }

    /// <summary>
    /// Updates author life time 
    /// </summary>
    /// <param name="birthYear">Year of birth</param>
    /// <param name="deathYear">Year of death</param>
    /// <param name="updatedAt">Date of update</param>
    /// <returns></returns>
    public PublicationAuthor SetLifeTime(int? birthYear, int? deathYear, DateTimeOffset updatedAt)
    {
        BirthYear = birthYear;
        DeathYear = deathYear;
        UpdatedAt = updatedAt;
        return this;
    }
}