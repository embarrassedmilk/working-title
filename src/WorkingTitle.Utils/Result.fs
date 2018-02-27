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
            | Message           msg -> app "Message: " ; app msg
            | Exception         e -> app "Exception message: " ; app e.Message
            | Object            o ->   app "Object: " ; sb.Append o |> ignore
            | DescribedObject   (d,o) ->  app "Description: " ; app d ; app "Object: " ; sb.Append o |> ignore

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