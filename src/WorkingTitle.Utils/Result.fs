//https://gist.github.com/mrange/aa9e0898492b6d384dd839bc4a2f96a1
namespace WorkingTitle.Utils

open System.Text
open System.Collections.Generic

module RResult = 
    [<RequireQualifiedAccess>]
    type RBad = 
        | Message           of string
        | Exception         of exn
        | Object            of obj
        | DescribedObject   of string*obj
        
        member x.DescribeTo (sb: StringBuilder) =
            let inline app (t : string) = sb.Append t |> ignore
            match x with 
            | Message           msg     -> app "Message: "              ; app msg
            | Exception         e       -> app "Exception message: "    ; app e.Message
            | Object            o       -> app "Object: "               ; sb.Append o |> ignore
            | DescribedObject   (d,o)   -> app "Description: "          ; app d ; app "Object: " ; sb.Append o |> ignore

    [<RequireQualifiedAccess>]
    type RBadTree = 
        | Leaf of RBad
        | Fork of RBadTree*RBadTree

        member x.Visit (visitor: RBad -> bool) : bool =
            let stack = Stack<RBadTree> 16
            let rec follow t =
                let inline pop () = 
                    if stack.Count > 0 then
                        follow (stack.Pop ())
                    else
                        true
                match t with
                | Leaf v -> visitor v && pop ()
                | Fork (l, r) -> stack.Push r; follow l
            follow x

        member x.Flatten () : RBad [] =
            let result = ResizeArray 16
            x.Visit (fun v -> result.Add v; true) |> ignore
            result.ToArray ()

        member x.Describe () : string =
            let sb = StringBuilder 16
            x.Visit (
                fun rbad ->
                    if sb.Length > 0 then sb.Append "; " |> ignore
                    rbad.DescribeTo sb
                    true
            ) |> ignore
            sb.ToString ()

        member x.Join o = Fork (x, o)

    [<RequireQualifiedAccess>]
    [<Struct>]
    [<StructuredFormatDisplay("{StructuredDisplayString}")>]
    type RResult<'T> =
        | Good of good : 'T
        | Bad  of bad  : RBadTree

        member x.StructuredDisplayString =
            match x with
            | Good good -> sprintf "Good (%A)" good
            | Bad  bad  -> sprintf "Bad (%A)"  bad

        override x.ToString() = x.StructuredDisplayString

    module RResult = 
        exception DerefException of RBadTree

        let inline rreturn v = RResult.Good v

        let inline rbind (f: 'T -> RResult<'U>) (r: RResult<'T>) : RResult<'U> =
            match r with
            | RResult.Good good -> f good
            | RResult.Bad  bad  -> RResult.Bad bad

        let inline rapply (r: RResult<'T>) (f: RResult<'T -> 'U>) : RResult<'U> =
            match f,r with
            | RResult.Bad  fBad   , RResult.Bad rBad   -> fBad.Join rBad |> RResult.Bad
            | RResult.Good fGood  , RResult.Good rGood -> rreturn(fGood rGood)
            | RResult.Bad  fBad   , _                  -> RResult.Bad fBad
            | _                   , RResult.Bad rBad   -> RResult.Bad rBad

        let inline rmap (f: 'T -> 'U) (r: RResult<'T>) : RResult<'U> =
            match r with
            | RResult.Good rgood -> f rgood |> rreturn
            | RResult.Bad  rbad  -> RResult.Bad rbad

        let inline rgood    v   = rreturn v
        let inline rbad     b   = RResult.Bad (RBadTree.Leaf b)
        let inline rmsg     msg = rbad (RBad.Message msg)
        let inline rexn     e   = rbad (RBad.Exception e)

    type RResult<'T> with
        static member inline (>>=)  (x, uf) = RResult.rbind    uf x
        static member inline (<*>)  (x, t)  = RResult.rapply   t x
        static member inline (|>>)  (x, m)  = RResult.rmap     m x