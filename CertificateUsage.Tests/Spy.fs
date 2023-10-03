module CertificateUsage.Tests.Spy

type Spy<'b, 'a when 'a: equality> (stub: 'a -> 'b) =
    let mutable callArgs = []
    let stub = stub

    member this.Args = callArgs

    member this.Function =
        fun (args: 'a) ->
            callArgs <- args :: callArgs
            stub args

    member this.CalledNTimes = fun n -> callArgs |> List.length = n
    member this.CalledOnce = this.CalledNTimes 1
    member this.NotCalled = this.CalledNTimes 0

    member this.CalledNTimesWith =
        fun n args ->
            callArgs
            |> List.filter (fun x -> x = args)
            |> List.length = n

    member this.CalledOnceWith = this.CalledNTimesWith 1
