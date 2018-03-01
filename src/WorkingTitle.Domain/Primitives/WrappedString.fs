module WrappedString
    open WorkingTitle.Utils
    open WorkingTitle.Utils.RResult

    let create canonicalize isValid ctor (s:string) = 
        match s with
        | null -> RResult.rmsg "input value must be provided"
        | _ ->
            let s' = canonicalize s
            if isValid s'
            then RResult.rgood (ctor s')
            else RResult.rmsg "validation failed"

    let apply f (s:string) = 
        s |> f

    let value s = apply id s

    let equals left right =
        (value left) = (value right)

    let compareTo left right =
        (value left).CompareTo (value right)
        
    let singleLineTrimmed s =
        System.Text.RegularExpressions.Regex.Replace(s,"\s"," ").Trim()