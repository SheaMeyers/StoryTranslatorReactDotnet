import AppBar from "@mui/material/AppBar"
import Button from "@mui/material/Button"
import "../styling/Header.css"

type HeaderProps = {
  apiToken: string
}

const Header = (props: HeaderProps) => {
  return (
    <AppBar position="static" className="Header">
      {props.apiToken ? (
        <>
          <Button variant="outlined" className="Header__Button">
            Logout
          </Button>
          <Button variant="outlined" className="Header__Button">
            Change Password
          </Button>
        </>
      ) : (
        <>
          <Button variant="outlined" className="Header__Button">
            Login
          </Button>
          <Button variant="outlined" className="Header__Button">
            Sign Up
          </Button>
        </>
      )}
    </AppBar>
  )
}

export default Header
