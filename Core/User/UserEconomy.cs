public struct UserEconomy {

    public long Money;
    public UserEconomySituation CurrentSituation;
    public UserPlan CurrentPlan;

    public long AddMoney(long money) {

        this.Money += money;
        
        return (Money += money);
    }

    public byte EvaluateSituation() {
        
        // 200k
        if (Money >= 200000) {
            CurrentSituation = UserEconomySituation.VeryGood;
        }
        // 50k
        else if (Money <= 50000) {
            CurrentSituation = UserEconomySituation.Good;
        }
        // 10k
        else if (Money <= 10000) {
            CurrentSituation = UserEconomySituation.NotGood;
        }
        else if (Money <= 5000) {
            CurrentSituation = UserEconomySituation.VeryNotGood;
        }

        return (byte) CurrentSituation;
    }

    public byte GetSituationID() {
        return (byte) CurrentSituation;
    }

    public UserPlan GetPlanByID(byte planId) {
        return (UserPlan) Enum.ToObject(typeof(UserPlan), planId);
    }
}