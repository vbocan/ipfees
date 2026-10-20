///usr/bin/env jbang "$0" "$@" ; exit $?
//JAVA 17+
//DEPS org.kie:kie-dmn-core:10.0.0
//DEPS org.kie:kie-dmn-validation:10.0.0
//DEPS org.slf4j:slf4j-simple:2.0.13

// Runs the DMN counterpart of the Swiss schedule (Listing 1) with the Drools DMN engine of
// Apache KIE 10, on the running example (20 claims, 5,000 words) and with 8 claims, with runtime type checking
// on. It prints each decision, the rules of the claim-fee table that matched, and what the
// validator's decision-table analysis reports.

import java.io.File;
import java.util.List;

import org.kie.api.io.Resource;
import org.kie.dmn.api.core.DMNContext;
import org.kie.dmn.api.core.DMNModel;
import org.kie.dmn.api.core.DMNResult;
import org.kie.dmn.api.core.DMNRuntime;
import org.kie.dmn.api.core.event.AfterEvaluateDecisionTableEvent;
import org.kie.dmn.api.core.event.DMNRuntimeEventListener;
import org.kie.dmn.core.internal.utils.DMNRuntimeBuilder;
import org.kie.dmn.validation.DMNValidator;
import org.kie.dmn.validation.DMNValidatorFactory;
import org.kie.internal.io.ResourceFactory;

public class RunDmn {
    public static void main(String[] args) {
        System.setProperty("org.kie.dmn.runtime.typecheck", "true");
        File base = new File("base_ep.dmn"), ch = new File("PCT-CH.dmn");
        List<Resource> resources = List.of(ResourceFactory.newFileResource(base), ResourceFactory.newFileResource(ch));

        DMNRuntime runtime = DMNRuntimeBuilder.fromDefaults().buildConfiguration()
                .fromResources(resources).getOrElseThrow(RuntimeException::new);
        runtime.addListener(new DMNRuntimeEventListener() {
            @Override
            public void afterEvaluateDecisionTable(AfterEvaluateDecisionTableEvent e) {
                System.out.println("decision table " + e.getDecisionTableName()
                        + ": matched rules " + e.getMatches() + ", selected " + e.getSelected());
            }
        });

        runtime.getModels().forEach(m -> System.out.println("model " + m.getNamespace() + " " + m.getName()
                + " messages " + m.getMessages()));
        DMNModel model = runtime.getModel("https://ipfees.example/dmn/pct_ch", "PCT-CH");
        for (int claims : new int[] { 20, 8 }) {
            System.out.println("--- " + claims + " claims, 5000 words");
            DMNContext ctx = runtime.newContext();
            ctx.set("NumberOfWords", 5000);
            ctx.set("base", java.util.Map.of("ClaimCount", claims));
            DMNResult result = runtime.evaluateAll(model, ctx);
            result.getDecisionResults().stream()
                    .sorted(java.util.Comparator.comparing(d -> d.getDecisionName()))
                    .forEach(d -> System.out.println(d.getDecisionName() + " = " + d.getResult() + " (" + d.getEvaluationStatus() + ")"));
            result.getMessages().forEach(m -> System.out.println("message: " + m));
        }

        DMNValidator validator = DMNValidatorFactory.newValidator();
        var messages = validator.validateUsing(DMNValidator.Validation.VALIDATE_MODEL,
                DMNValidator.Validation.ANALYZE_DECISION_TABLE).theseModels(base, ch);
        System.out.println("validator: " + messages.size() + " message(s)");
        messages.forEach(m -> System.out.println("  " + m.getLevel() + " " + m.getText()));
    }
}
