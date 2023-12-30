import Button from '@mui/material/Button'
import Modal from '@mui/material/Modal'
import Box from '@mui/material/Box'
import { useState } from 'react'
import { logout } from '../../apis/UserApi'
import Checkbox from '@mui/material/Checkbox'
import FormGroup from '@mui/material/FormGroup'
import FormControlLabel from '@mui/material/FormControlLabel'
import '../../styling/Modals.css'


type LogoutModalProps = {
  isOpen: boolean
  apiToken: string
  handleClose: (loggedOut?: boolean) => void
}


const LogoutModal = (props: LogoutModalProps) => {
  const [logoutAll, setLogoutAll] = useState<boolean>(false)

  const handleSubmit = async () => {
    await logout(props.apiToken, logoutAll)
    props.handleClose(true)
  } 
  
  return (
    <Modal
      open={props.isOpen}
      onClose={() => props.handleClose()}
      aria-labelledby="modal-modal-title"
      aria-describedby="modal-modal-description"
    >
      <Box className="Modal__Box">
        <FormGroup>
          <FormControlLabel 
            control={
              <Checkbox 
                value={logoutAll} 
                onClick={() => setLogoutAll(!logoutAll)}
              />
            } 
            label="Logout from all devices?" 
          />
        </FormGroup>
        <Button variant="contained" onClick={handleSubmit}>Logout</Button>
        <Button variant="contained" onClick={() => props.handleClose()}>Close</Button>
      </Box>
    </Modal>
  );
}

export default LogoutModal
