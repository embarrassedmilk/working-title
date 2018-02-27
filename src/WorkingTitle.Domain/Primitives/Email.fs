namespace WorkingTitle.Domain.Primitives

module EmailAddress = 
    type T = EmailAddress of string with
        interface WrappedString.IWrappedString with
            member this.Value = let (EmailAddress s) = this in s

    let create =
        let canonicalize = WrappedString.singleLineTrimmed
        let isValid s = System.Text.RegularExpressions.Regex.IsMatch(s,@"^\S+@\S+\.\S+$")

        WrappedString.create canonicalize isValid EmailAddress

    let convert s = WrappedString.apply create s