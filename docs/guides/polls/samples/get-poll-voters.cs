// Get the id of the first answer in the poll
var answerId = message.Poll.Answers.First().AnswerId;
// Get the list of voters who voted for the first answer
var voters = await message.GetPollAnswerVotersAsync(answerId).FlattenAsync();