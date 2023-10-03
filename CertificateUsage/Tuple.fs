module CertificateUsage.Tuple

open FsToolkit.ErrorHandling

let traverseValidationFst f (a: Validation<'a, 'e>, b: 'b) =
    Validation.map (fun a' -> (f a', b)) a

let pipe (a, b) p = (a p, b p)
