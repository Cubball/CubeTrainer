import CasesTable from '../components/CasesTable'
import Tabs from '../components/Tabs'

const MyCases = () => {
  return (
    <div className="flex h-full w-full flex-col items-center gap-4">
      <h1 className="text-2xl font-bold">My Cases</h1>
      <p>
        Here you can see a list of all your cases, their status and selected
        algorithm
      </p>
      <Tabs
        tabs={[
          {
            name: 'OLL',
            element: (
              <CasesTable
                cases={[
                  {
                    id: '55168995-c2e9-4fea-8586-3904a1d4e68a',
                    name: 'OLL 1',
                    status: 'InProgress',
                    scramble: "R' U' F R' F' R U R",
                    selectedAlgorithmMoves:
                      "R U R' U' R' F R2 U' R' U' R U R' F'",
                  },
                  {
                    id: '704f19b7-2a35-4654-a8ad-e05d0be4a649',
                    name: 'OLL 2',
                    status: 'InProgress',
                    scramble: "R U2 R' U' R U' R'",
                    selectedAlgorithmMoves: "R U R' U R U2 R'",
                  },
                ]}
              />
            ),
          },
          { name: 'PLL', element: <div>PLL</div> },
        ]}
      />
    </div>
  )
}

export default MyCases
