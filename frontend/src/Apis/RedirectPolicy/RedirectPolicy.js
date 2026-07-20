

  export function Redirect(status,isRedirecting=false) {

     if (
            status === 401 &&
            !isRedirecting
        ) {

              localStorage.setItem(
                "authMessage",
                "Your session has expired. Please sign in again."
            );
            window.location.href =
                "/signin-options";
        }
  }