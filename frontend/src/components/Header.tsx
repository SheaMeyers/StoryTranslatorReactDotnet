import AppBar from '@mui/material/AppBar'
import Button from '@mui/material/Button'
import "../styling/Header.css"

const Header = () => {
  return (
    <AppBar position="static" className="Header" >
        <Button variant="outlined" className="Header__Button">Login</Button>
        <Button variant="outlined" className="Header__Button">Sign Up</Button>
    </AppBar>
  );
}

export default Header;
