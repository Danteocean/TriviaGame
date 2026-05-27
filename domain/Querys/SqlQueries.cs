namespace Domain.Querys;

public class SqlQueries
{

    // Obtiene una pregunta aleatoria basada en el nivel actual de la sesión
    public static string GetRandomQuestionByLevel => @"
        SELECT TOP 1 
            Q.Id AS QuestionId, 
            Q.Text 
        FROM Questions Q
        INNER JOIN Categories C ON Q.CategoryId = C.Id
        WHERE C.DifficultyLevel = @Level
        ORDER BY NEWID()";

    // Obtiene todas las opciones asociadas a una pregunta específica
    public static string GetOptionsByQuestion => @"
        SELECT 
            Id AS OptionId, 
            Text 
        FROM Options 
        WHERE QuestionId = @QuestionId";

    // Verifica si la opción seleccionada por el usuario es la correcta
    public static string CheckIfAnswerIsCorrect => @"
        SELECT 
            IsCorrect 
        FROM Options 
        WHERE Id = @OptionId";

    // Query solicitado anteriormente para obtener la sesión
    public static string GetGameSessionById => @"
        SELECT 
            Id, PlayerId, CurrentRound, AccumulatedPrize, IdStatus, CreatedAt 
        FROM GameSessions 
        WHERE Id = @Id";

    public static string GetPlayerById => @"
        SELECT 
            Id, Alias, TotalPointsAchieved 
        FROM Player 
        WHERE Id = @Id";

    public static string GetPlayerByAlias => @"
        SELECT 
            Id, Alias, TotalPointsAchieved 
        FROM Player 
        WHERE Alias = @Alias";

    public static string GetAllQuestions => @"
        SELECT 
            Q.Id AS QuestionId, Q.Text, Q.CategoryId,C.Points , O.Id AS OptionId, O.Text, O.IsCorrect
            FROM Questions Q
            LEFT JOIN Options O ON Q.Id = O.QuestionId
            LEFT JOIN Categories C ON Q.CategoryId = C.Id";

    public static string GetAllCategories => @"
    SELECT Id, Name, DifficultyLevel 
    FROM Categories";

    public static string GetTopScores => @"
    SELECT TOP 10 
        P.Alias, 
        S.AccumulatedPrize, 
        S.CreatedAt
    FROM GameSessions S
    INNER JOIN Players P ON S.PlayerId = P.Id
    WHERE S.Status = 'Won'
    ORDER BY S.AccumulatedPrize DESC";

    public static string GetPoints => @"
        SELECT 
            Points 
        FROM Categories 
        WHERE DifficultyLevel = @Level";
}
