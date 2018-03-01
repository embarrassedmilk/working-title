namespace WorkingTitle.Domain.Primitives

module EmailAddress = 
    let create =
        let canonicalize = WrappedString.singleLineTrimmed
        let isValid s = System.Text.RegularExpressions.Regex.IsMatch(s,@"^\S+@\S+\.\S+$")

        WrappedString.create canonicalize isValid string