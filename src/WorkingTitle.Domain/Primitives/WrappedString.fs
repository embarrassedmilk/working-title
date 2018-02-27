module WrappedString
    open WorkingTitle.Utils

    type IWrappedString = 
        abstract Value: string

    let create canonicalize isValid ctor (s:string) = 
        match s with
        | null -> Result.F
        | _ ->
            let s' = canonicalize s
            if isValid s'
            then Some (ctor s')
            else None

    let apply f (s:IWrappedString) = 
        s.Value |> f

    let value s = apply id s

    let equals left right =
        (value left) = (value right)

    let compareTo left right =
        (value left).CompareTo (value right)
        
    let singleLineTrimmed s =
        System.Text.RegularExpressions.Regex.Replace(s,"\s"," ").Trim()