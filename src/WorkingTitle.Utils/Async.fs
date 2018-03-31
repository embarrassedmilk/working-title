namespace WorkingTitle.Utils

module Async = 
    let map f xAsync = async {
        let! x = xAsync

        return f x
    }

    let bind f xAsync = async {
        let! x = xAsync

        return! f x
    }

    let apply fAsync xAsync = async {
        let! x = xAsync
        let! f = fAsync
        return f x
    }

    let retn x = async {
        return x
    }