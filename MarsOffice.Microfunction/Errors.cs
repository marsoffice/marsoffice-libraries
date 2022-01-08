using System;
using System.Collections.Generic;
using System.Linq;

namespace MarsOffice.Microfunction
{
    public static class Errors
    {
        public static Dictionary<string, List<ErrorDto>> Extract(Exception ex)
        {
            var result = new Dictionary<string, List<ErrorDto>>();
            if (ex == null)
            {
                var errors = new List<ErrorDto> {
                    new ErrorDto {
                        Message = "errors.unkownError"
                    }
                };
                result[string.Empty] = errors;
                return result;
            }
            AddExceptionDataToResult(ex, result);
            foreach (var kvp in result)
            {
                foreach (var err in kvp.Value)
                {
                    if (string.IsNullOrEmpty(err.Message) || !err.Message.Contains("|"))
                    {
                        continue;
                    }
                    var split = err.Message.Split("|");
                    if (split.Length < 2)
                    {
                        continue;
                    }
                    err.Message = split[0];
                    err.PlaceholderValues = new Dictionary<string, string>();
                    var splitBySemicolon = split[1].Split(";");
                    foreach (var placeholderTuple in splitBySemicolon)
                    {
                        var splitByColon = placeholderTuple.Split(":");
                        if (splitByColon.Length < 2)
                        {
                            continue;
                        }
                        err.PlaceholderValues[splitByColon[0]] = splitByColon[1];
                    }
                }
            }
            return result;
        }

        private static void AddExceptionDataToResult(Exception e, Dictionary<string, List<ErrorDto>> errorMessages)
        {
            switch (e.Source)
            {
                case "FluentValidation":
                    var splitErrors = e.Message.Split(" -- ");
                    if (splitErrors.Length < 2)
                    {
                        if (errorMessages.ContainsKey(string.Empty))
                        {
                            errorMessages[string.Empty] = new List<ErrorDto>();
                        }
                        errorMessages[string.Empty].Add(new ErrorDto
                        {
                            Message = "gateway.exceptionHandler.validationError"
                        });
                        break;
                    }
                    var validationLines = splitErrors.Skip(1).Select(x => x.Split(": ")).Select(x => (x[0], x[1].Split(" ")[0])).ToList();
                    foreach (var validationLine in validationLines)
                    {
                        if (!errorMessages.ContainsKey(validationLine.Item1))
                        {
                            errorMessages[validationLine.Item1] = new List<ErrorDto>();
                        }
                        errorMessages[validationLine.Item1].Add(new ErrorDto
                        {
                            Message = validationLine.Item2
                        });
                    }
                    break;

                default:
                    if (!errorMessages.ContainsKey(string.Empty))
                    {
                        errorMessages[string.Empty] = new List<ErrorDto>();
                    }
                    errorMessages[string.Empty].Add(new ErrorDto
                    {
                        Message = e.Message
                    });
                    break;
            }

        }
    }
}