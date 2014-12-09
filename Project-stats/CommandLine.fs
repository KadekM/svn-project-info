module CommandLine

(*
-ssr / --set-svn-repository - svn repository

*)

type SvnRepositoryOption = | SvnRepositoryOption of string

type CommandLineOptions = {
    svn: Option<SvnRepositoryOption>;
    from: Option<System.DateTimeOffset>;
    until: Option<System.DateTimeOffset>;
}

let defaultCommandLineOptions = { svn = None; from = None; until = None }

let (|Prefix|_|) (p: string) (s: string) =
    if s.StartsWith(p) then Some(s.Substring(p.Length))
    else None

let rec parseCommandLineRec args options =
    match args with
    | x::xs ->
        match x with
        | Prefix "-ssr=" url | Prefix "--set-svn-repository=" url -> {options with svn=Some(SvnRepositoryOption(url)) } |> parseCommandLineRec xs
        | Prefix "-f=" date | Prefix "--from=" date ->
             match System.DateTimeOffset.TryParse date with
             | true, parsed -> {options with from=Some(parsed) } |> parseCommandLineRec xs
             | false, _ -> printfn "Invalid date %A" date
                           options
        | Prefix "-u=" date | Prefix "--until=" date ->
             match System.DateTimeOffset.TryParse date with
             | true, parsed -> {options with from=Some(parsed) } |> parseCommandLineRec xs
             | false, _ -> printfn "Invalid date %A" date
                           options
        | arg -> printfn "Argument %s is not known" arg
                 options |> parseCommandLineRec xs
    | _ -> options

        
        