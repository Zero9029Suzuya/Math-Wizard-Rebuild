Rule 1
	Never commit directly to main.

Rule 2
	Every task starts from develop branch.
		git switch develop
		git pull
		git switch -c feature/[thing you want to develop]
			ex. git switch -c feature/player-controller

Rule 3
	Only the feature owner works on their branch
		If you want to collaborate create a separate branch.

Rule 4
	Finished a feature?
	Open a Pull Request to develop, or if you are not using pull request consistently, ask to review
	before merging.
	
Rule 5
	Only the lead can merge develop into main.
	
Habitualize the following:
	git switch develop
	git pull
	
	at the end of the day:
	git push