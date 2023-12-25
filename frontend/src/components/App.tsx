import { useEffect, useState } from 'react'
import { getUsernameAndToken } from '../api'
import Header from './Header'


const App = () => {
  const [username, setUsername] = useState<string>('')
  const [apiToken, setApiToken] = useState<string>('')

  useEffect(() => {
    const fetchData = async () => {
      debugger
      const [retrievedUsername, retrievedApiToken] = await getUsernameAndToken()
      console.log('retrievedUsername')
      console.log(retrievedUsername)
      console.log('retrievedApiToken')
      console.log(retrievedApiToken)
      debugger
      if (retrievedUsername && retrievedApiToken) {
        setUsername(retrievedUsername)
        setApiToken(retrievedApiToken)
      }
    }
    fetchData()  
  }, [])

  return (
    <>
     <Header apiToken={apiToken} username={username} /> 
    </>
  )
}

export default App
